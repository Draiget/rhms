using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.CPU;
using server.Modules.Base;

namespace cpu_intel.Api.Hardware.Sensors.Base
{
    public abstract class SensorBaseIntelCpuMulti : BaseCpuSensor, IMultiValueSensor
    {
        protected new IntelCpu Cpu;

        protected SensorBaseIntelCpuMulti(IntelCpu cpu)
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
        
        public override BaseModule GetModuleHandle() {
            return ModuleCpuIntel.ModuleHandle;
        }

        public abstract ISensorElement[] GetElements();

        public override string ToString() {
            return $"BaseIntelMultiSensor [Type={GetSensorType()}, Name={GetDisplayName()}]";
        }
    }
}
