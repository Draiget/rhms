using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking
{
    public class NetworkClient
    {
        private Socket _socket;

        // TODO: Hole punching

        public NetworkClient(){
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void RegisterOnServer(){
            
        }
    }
}
