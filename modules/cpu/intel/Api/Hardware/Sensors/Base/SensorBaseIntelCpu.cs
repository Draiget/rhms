using System;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.CPU;
using server.Modules.Base;

namespace cpu_intel.Api.Hardware.Sensors.Base
{
    public abstract class SensorBaseIntelCpu : BaseCpuSensor, ISingleValueSensor
    {
        protected new GenericCpu Cpu;

        protected SensorBaseIntelCpu(GenericCpu cpu)
            : base(cpu) {
            Cpu = cpu;
        }

        public override bool InitSensor() {
            return true;
        }

        public override void Tick() {
            TickSpecificLoad(Cpu.GetCpuidProcessorInfo());
        }

        public abstract void TickSpecificLoad(CpuidProcessorInfo info);

        public override SensorType GetSensorType() {
            return SensorType.Utilization;
        }

        public override BaseModule GetModuleHandle() {
            return ModuleCpuIntel.ModuleHandle;
        }

        public abstract ISensorElement GetElement();
    }
}
