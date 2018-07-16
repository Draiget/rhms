using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Control.Packets
{
    [NetPacketRegister(1)]
    public class PacketCPing : NetPacket
    {
        private string _tmp;

        public override void Read(StreamReader sr, RemotePacketState state) {
            var input = sr.ReadToEnd();
            if (input.StartsWith("ping")) {
                _tmp = "pong";
            }
        }

        public override void Write(StreamWriter sw, RemotePacketState state) {
            sw.Write(_tmp);
        }

        public override void Process(NetPacketHandler handler) {
            handler.ProcessPingPacket(this);
        }
    }
}
