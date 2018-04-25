using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Utils;

namespace server.Networking
{
    public class NetworkConnectionManager
    {
        public const int NetPingInterval = 10000;
        private const int NetPingThreadInterval = 2000;

        private readonly Thread _networkThread;
        private readonly NetworkClient _networkClient;
        private readonly RhmsCollectingServer _collectingServer;
        private bool _isRequestShutdown;

        public NetworkConnectionManager(RhmsCollectingServer server) {
            _isRequestShutdown = false;
            _collectingServer = server;
            _networkClient = new NetworkClient(server, this);
            _networkThread = new Thread(WorkerUpdateNetworkThread);
        }

        private void WorkerUpdateNetworkThread() {
            _networkClient.Initialize();

            while (!_isRequestShutdown) {
                _networkClient.UpdateNetwork();
                Thread.Sleep(NetPingThreadInterval);
            }
        }

        public void Initialize() {
            _networkThread.Name = "network_updater";
            _networkThread.Start();
        }

        public string GetSelfHostId() {
            return _collectingServer.GetSettings().CollectingServerPeerName;
        }

        public void Shutdown() {
            _isRequestShutdown = true;

            try {
                _networkClient.OnShutdown();
            } catch {
                ;
            }

            ThreadUtils.JoinIgnoreErrors(_networkThread);
        }
    }
}
