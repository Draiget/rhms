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
        public override void Process(StreamReader sr, RemotePacketState state) {
            Logger.Info("Receive reboot packet!");
        }

        public override void Response(StreamWriter sw, RemotePacketState state) {
            
        }
    }
}
