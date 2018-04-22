using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using server.Networking.Stun;
using server.Utils.Logging;

namespace server.Networking
{
    public class NetworkClient
    {
        private readonly Socket _socket;

        public NetworkClient(){
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Initialize() {
            Logger.Info("Initializing networking");

            try {
                CheckNatType();
            } catch (Exception e) {
                Logger.Error("Unable to initialize networking!", e);
            }
        }

        // STUN server list: https://gist.github.com/yetithefoot/7592580

        public void CheckNatType() {
            var type = StunQuery.Perform(_socket, "stun.l.google.com", 19302);
            Logger.Info($"NAT status: {type.NetworkType} [address={type.PublicEndPoint}]");
        }
    }
}
