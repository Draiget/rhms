using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Open.Nat;
using server.Addons;
using server.Networking.Stun;
using server.Utils;
using server.Utils.Logging;

namespace server.Networking
{
    public class NetworkClient
    {
        

        private Socket _socket;
        private StunQueryResult _stunQuery;
        private readonly RhmsCollectingServer _collectingServer;
        private readonly NetworkConnectionManager _connectionManager;
        private readonly IPEndPoint _bindEp;
        private IPEndPoint _remoteEp;

        private string _peerControlHost;
        private long _lastPeerPingTime;
        private long _lastInternalErrorTime;
        private int _internalErrorTimes;

        public NetworkClient(RhmsCollectingServer server, NetworkConnectionManager connectionManager) {
            _connectionManager = connectionManager;
            _collectingServer = server;
            _peerControlHost = _collectingServer.GetSettings().PeerSignalServer.Host;
            _bindEp = new IPEndPoint(IPAddress.Parse(_collectingServer.GetSettings().BindAddress), 0);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(_bindEp);

            if (_peerControlHost.EndsWith("/")) {
                _peerControlHost = _peerControlHost.Substring(0, _peerControlHost.Length - 1);
            }
        }

        public void Initialize() {
            Logger.Info("Initializing networking");
            
            while (true) {
                try {
                    _stunQuery = null;
                    CheckNatType();

                    if (_stunQuery != null) {
                        break;
                    }
                } catch (Exception e) {
                    Logger.Error("Unable to initialize networking! Retrying", e);
                    Thread.Sleep(5000);
                }
            }

            _remoteEp = _stunQuery.PublicEndPoint;
            var internalPort = ((IPEndPoint)_socket.LocalEndPoint).Port;

            Logger.Debug($"P2P connections are listen as external {_stunQuery.PublicEndPoint.Address}:{_stunQuery.PublicEndPoint.Port}, local bind is {_bindEp.Address}:{internalPort}");
            Logger.Info($"Bound local socket at {(IPEndPoint) _socket.LocalEndPoint}");

            Task.Run(async () => {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                Logger.Info($"Open.NAt external ip: {device.GetExternalIPAsync().Result}");
                await device.CreatePortMapAsync(new Mapping(Protocol.Udp, internalPort, _stunQuery.PublicEndPoint.Port, $"RHMS Internal Peer {_connectionManager.GetSelfHostId()} Map"));

                Logger.Info($"Create UPnP mapping {_stunQuery.PublicEndPoint.Port} -> {internalPort}");
            });

            // _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // _socket.Bind(_bindEp);
            NetworkStartAcceptingData();
        }

        private void NetworkStartAcceptingData() {
            var container = new PacketDataContainer();
            _socket.BeginReceiveFrom(container.Buffer, 0, container.Buffer.Length, SocketFlags.None, ref container.Remote, NetworkAcceptDataAsync, container);
        }

        private void NetworkAcceptDataAsync(IAsyncResult result) {
            try {
                var container = (PacketDataContainer) result.AsyncState;
                var len = _socket.EndReceiveFrom(result, ref container.Remote);
                if (len == 51) {
                    Logger.Info($"Received data packet from [/{container.Remote}], len: {len}");
                }
            } catch (Exception e) {
                Logger.Error("Accept data packet error", e);
            } finally {
                NetworkStartAcceptingData();
            }
        }

        // STUN server list: https://gist.github.com/yetithefoot/7592580

        public void CheckNatType() {
            _stunQuery = StunQuery.Perform(_socket, "stun.l.google.com", 19302);
            if (_stunQuery == null) {
                return;
            }

            Logger.Info($"NAT status: {_stunQuery.NetworkType} [address={_stunQuery.PublicEndPoint}]");
        }

        private void WaitInternalErrors() {
            // Wait more if server failed with error
            var secondsTimeout = MathUtils.Clamp(10 * _internalErrorTimes, 0, 60 * 10);
            if (DateTime.Now.Ticks - _lastInternalErrorTime < TimeSpan.TicksPerSecond * secondsTimeout) {
                Logger.Warn($"Setting requests timeout of {secondsTimeout} seconds due to latest InternalServerError from peer discovery service");
                Thread.Sleep(secondsTimeout * 1000);
            }
        }

        private void RegisterPeer() {
            WaitInternalErrors();

            var peerName = _connectionManager.GetSelfHostId();
            var objResponse = HttpUtils.SendGet($"{_peerControlHost}/udp-peer/register", $"name={peerName}&port={_remoteEp.Port}", out var response);
            if (response.StatusCode != HttpStatusCode.OK) {
                if (objResponse == null) {
                    Logger.Warn($"Failed to register this collecting server on peer discovery service, http code: {response.StatusCode}");
                    return;
                }

                if (response.StatusCode == HttpStatusCode.InternalServerError) {
                    _lastInternalErrorTime = DateTime.Now.Ticks;
                    _internalErrorTimes++;
                } else {
                    _internalErrorTimes = 0;
                }

                var errorObj = objResponse["errors"].First;
                Logger.Warn($"Failed to register this collecting server on peer discovery service: {errorObj["message"]} [httpCode={response.StatusCode}]");
                return;
            }

            _internalErrorTimes = 0;
            _lastPeerPingTime = DateTime.Now.Ticks;
            Logger.Info("Collecting server successfully registered in peer discovery service");
        }

        public void UpdateNetwork() {
            if (DateTime.Now.Ticks - _lastPeerPingTime < TimeSpan.TicksPerMillisecond * NetworkConnectionManager.NetPingInterval) {
                return;
            }

            if (_remoteEp == null) {
                return;
            }

            WaitInternalErrors();
            var obj = HttpUtils.SendGet($"{_peerControlHost}/udp-peer/ping", $"port={_remoteEp.Port}", out var response);
                
            if (response.StatusCode == HttpStatusCode.ExpectationFailed) {
                RegisterPeer();
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK) {
                if (response.StatusCode == HttpStatusCode.InternalServerError) {
                    Logger.Warn("Peer discovery service return internal server error");
                } else {
                    Logger.Warn($"Unknown response from peer discovery service, http code: {response.StatusCode}");
                }

                _lastInternalErrorTime = DateTime.Now.Ticks;
                _internalErrorTimes++;
                return;
            }

            _internalErrorTimes = 0;
            _lastPeerPingTime = DateTime.Now.Ticks;
        }

        internal class PacketDataContainer
        {
            public byte[] Buffer;
            public EndPoint Remote;

            public PacketDataContainer() {
                Remote = new IPEndPoint(IPAddress.Any, 0);
                Buffer = new byte[2048];
            }
        }
    }
}
