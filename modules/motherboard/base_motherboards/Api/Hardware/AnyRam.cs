using System;
using System.Runtime.InteropServices;
using server.Hardware;
using server.Hardware.CPU;
using server.Hardware.RAM;
using server.Utils;
using server.Utils.Natives;

namespace base_motherboards.Api.Hardware
{
    public class AnyRam : GenericRam
    {
        private readonly AnyRamIdentifier _identifier;

        public AnyRam() {
            _identifier = new AnyRamIdentifier(this);
        }

        public override HardwareIdentifer Identify() {
            return _identifier;
        }

        public override void InitializeSensors() {
            var sensors = AssemblyUtils.GetAttributesInModule(typeof(AnyRam), typeof(SensorRegisterAttribute));
            foreach (var sensor in sensors) {
                AddAvaliableSensor((BaseRamSensor)Activator.CreateInstance(sensor, this));
            }
        }

        public override void TickUpdate() {
            var sensors = GetSensors();
            foreach (var sensor in sensors) {
                if (sensor.IsAvaliable()) {
                    sensor.Tick();
                }
            }
        }

        public override string ToString() {
            return "AnyRam [<No extended data, data obtain using WMI>]";
        }

        public MemoryStatusEx? GetMemoryStatusInfo() {
            var status = new MemoryStatusEx {
                Length = checked((uint)Marshal.SizeOf(typeof(MemoryStatusEx)))
            };

            if (!Native.GlobalMemoryStatusEx(ref status)) {
                return null;
            }

            return status;
        }
    }
}
