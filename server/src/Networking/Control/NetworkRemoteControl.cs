using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;
using server.Hardware.GPU;
using server.Utils;
using server.Utils.Logging;

namespace server.Networking.Control
{
    public class NetworkRemoteControl
    {
        private static Dictionary<uint, Type> _packetsToTypesMap;
        private static Dictionary<Type, uint> _typesToPacketsMap;

        public static NetworkClient ClientInstance {
            get;
            set;
        }

        public static void Initialize() {
            _packetsToTypesMap = new Dictionary<uint, Type>();
            _typesToPacketsMap = new Dictionary<Type, uint>();

            var packets = AssemblyUtils.GetAttributesInModule(typeof(NetworkRemoteControl), typeof(NetPacketRegisterAttribute));
            foreach (var packet in packets) {
                var attribute = packet.GetCustomAttribute<NetPacketRegisterAttribute>();
                if (attribute == null) {
                    continue;
                }

                if (_packetsToTypesMap.ContainsKey(attribute.PacketId)) {
                    throw new Exception($"Packet with id '{attribute.PacketId}' already registered!");
                }

                _packetsToTypesMap.Add(attribute.PacketId, packet);
                _typesToPacketsMap.Add(packet, attribute.PacketId);
            }
        }

        public static void AcceptPacket(RemotePacketState remoteState, NetworkClient.PacketDataContainer packetData) {
            var packetBuffer = new byte[packetData.Buffer.Length - 5];
            Buffer.BlockCopy(packetData.Buffer, 5, packetBuffer, 0, packetBuffer.Length);

            var packetId = BitConverter.ToUInt32(packetData.Buffer, 1);
            Type packetType;
            if ((packetType = FindPacketWithId(packetId)) == null) {
                // Unknown packet
                Logger.Debug($"Unknown packet id '{packetId}' from [/{packetData.Remote as IPEndPoint}]");
                return;
            }

            NetPacket netPacket;
            try {
                netPacket = (NetPacket) Activator.CreateInstance(packetType);
            } catch (Exception e) {
                Logger.Error($"Unable to create instance of packet id '{packetId}'", e);
                return;
            }

            try {
                using (var sr = new StreamReader(new MemoryStream(packetBuffer))) {
                    netPacket.Process(sr, remoteState);
                    using (var ms = new MemoryStream()) {
                        using (var bw = new BinaryWriter(ms)) {
                            bw.Write(packetId);
                            using (var sw = new StreamWriter(ms)) {
                                netPacket.Response(sw, remoteState);
                                ClientInstance?.SendPacket(ms.GetBuffer(), packetData);
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Logger.Error($"Unable to process packet {netPacket}");
            }
        }

        public static Type FindPacketWithId(uint id) {
            if (_packetsToTypesMap.ContainsKey(id)) {
                return _packetsToTypesMap[id];
            }

            return null;
        }

        public static uint? FindPacketIdByType(Type packetType) {
            if (_typesToPacketsMap.ContainsKey(packetType)) {
                return _typesToPacketsMap[packetType];
            }

            return null;
        }
    }
}
