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
        public abstract void Read(StreamReader sr, RemotePacketState state);

        public abstract void Write(StreamWriter sw, RemotePacketState state);

        public abstract void Process(NetPacketHandler handler);

        public override string ToString() {
            return $"NetPacket [Id={NetworkRemoteControl.FindPacketIdByType(GetType())}, Type={GetType().Name}]";
        }
    }
}
