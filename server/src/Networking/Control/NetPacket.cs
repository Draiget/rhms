using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Control
{
    public abstract class NetPacket
    {
        public abstract void Process(StreamReader sr, RemotePacketState state);

        public abstract void Response(StreamWriter sw, RemotePacketState state);

        public override string ToString() {
            return $"NetPacket [Id={NetworkRemoteControl.FindPacketIdByType(GetType())}, Type={GetType().FullName}]";
        }
    }
}
