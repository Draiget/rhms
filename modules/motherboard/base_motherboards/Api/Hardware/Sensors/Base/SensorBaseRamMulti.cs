using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.RAM;
using server.Modules.Base;
using server.Utils.Natives;

namespace base_motherboards.Api.Hardware.Sensors.Base
{
    public abstract class SensorBaseRamMulti : BaseRamSensor, IMultiValueSensor
    {
        protected new AnyRam Ram;

        protected SensorBaseRamMulti(AnyRam ram)
            : base(ram) {
            Ram = ram;
        }

        public override bool InitSensor() {
            return true;
        }

        public override void Tick() {
            TickSpecificLoad(Ram.GetMemoryStatusInfo());
        }

        public abstract void TickSpecificLoad(MemoryStatusEx? info);

        public override BaseModule GetModuleHandle() {
            return ModuleBaseMotherboards.ModuleHandle;
        }

        public abstract ISensorElement[] GetElements();

        public override string ToString() {
            return $"BaseRamMultiSensor [Type={GetSensorType()}, Name={GetDisplayName()}]";
        }
    }
}
