using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Stun
{
    public static class StunQuery
    {
        public const int StunTimeout = 4000;

        public static StunQueryResult Perform(Socket sock, string srv, int port) {
            if (port < 0 || port > ushort.MaxValue) {
                throw new ArgumentException($"Invalid server port - '{port}'", nameof(port));
            }

            var remoteAddr = Dns.GetHostAddresses(srv).FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            if (remoteAddr == null) {
                throw new ArgumentException($"Invalid server hostname - '{srv}'", nameof(srv));
            }

            var stunAddress = new IPEndPoint(remoteAddr, port);

            sock.ReceiveTimeout = StunTimeout;
            sock.SendTimeout = StunTimeout;

            return StartPerformingStunQuery(sock, stunAddress);
        }

        private static StunQueryResult StartPerformingStunQuery(Socket sock, IPEndPoint stunEp) {
            var request = new StunMessage {
                Type = StunMessageType.BindingRequest
            };

            var response = PerformStunTransaction(sock, stunEp, request);
            if (response == null) {
                return new StunQueryResult(StunNetworkType.UdpBlocked);
            }

            request.ChangeRequest = new StunChangeRequest(changeIp: true, changePort: true);

            // No active NAT in network
            if (sock.LocalEndPoint.Equals(response.MappedAddress)) {
                var typeTestRespons = PerformStunTransaction(sock, stunEp, request);
                if (typeTestRespons != null) {
                    // No NAT
                    return new StunQueryResult(StunNetworkType.OpenInternet, response.MappedAddress);
                }

                // Symmetric UDP firewall
                return new StunQueryResult(StunNetworkType.SymmetricUdpFirewall, response.MappedAddress);
            }

            // We have active NAT, check it's type
            var natTypeResponse = PerformStunTransaction(sock, stunEp, request);
            if (natTypeResponse != null) {
                // Full cone NAT
                return new StunQueryResult(StunNetworkType.FullCone, response.MappedAddress);
            }

            var test2Response = PerformStunTransaction(sock, stunEp, request);
            if (test2Response == null) {
                throw new Exception("STUN server didn't response at test two");
            }

            if (!test2Response.MappedAddress.Equals(response.MappedAddress)) {
                // Symmetric NAT
                return new StunQueryResult(StunNetworkType.Symmetric, response.MappedAddress);
            }

            request.ChangeRequest = new StunChangeRequest(changeIp: false, changePort: true);

            var test3Response = PerformStunTransaction(sock, stunEp, request);
            if (test3Response != null) {
                // Resticted cone NAT
                return new StunQueryResult(StunNetworkType.RestrictedCone, response.MappedAddress);
            }

            // Port-resticted cone NAT
            return new StunQueryResult(StunNetworkType.PortRestrictedCone, response.MappedAddress);
        }

        private static StunMessage PerformStunTransaction(Socket sock, IPEndPoint stunEp, StunMessage request) {
            var bytes = request.ToByteData();
            var startTime = DateTime.Now;
            while (startTime.AddSeconds(2) > DateTime.Now) {
                try {
                    sock.SendTo(bytes, stunEp);
                    if (sock.Poll(100, SelectMode.SelectRead)) {
                        var recvBuffer = new byte[512];
                        sock.Receive(recvBuffer);

                        var response = new StunMessage();
                        response.Parse(recvBuffer);

                        if (request.TransactionId.Equals(response.TransactionId)) {
                            return response;
                        }
                    }
                } catch {
                    ;
                }
            }

            return null;
        }
    }
}
