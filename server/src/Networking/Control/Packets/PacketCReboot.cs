using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Utils.Logging;

namespace server.Networking.Control.Packets
{
    [NetPacketRegister(2)]
    public class PacketCReboot : NetPacket
    {
        public override void Read(StreamReader sr, RemotePacketState state) {
            
        }

        public override void Write(StreamWriter sw, RemotePacketState state) {
            
        }

        public override void Process(NetPacketHandler handler) {
            handler.ProcessRebootPacket(this);
        }
    }
}
