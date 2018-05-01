using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Control
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NetPacketRegisterAttribute : Attribute
    {
        public uint PacketId {
            get;
        }

        public NetPacketRegisterAttribute(uint packetId) {
            PacketId = packetId;
        }
    }
}
