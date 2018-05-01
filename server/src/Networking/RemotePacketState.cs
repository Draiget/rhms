using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking
{
    public class RemotePacketState
    {
        public byte MagicByte {
            get;
        }

        public RemotePacketState() {
            MagicByte = (byte)new Random().Next(byte.MinValue, byte.MaxValue);
        }
    }
}
