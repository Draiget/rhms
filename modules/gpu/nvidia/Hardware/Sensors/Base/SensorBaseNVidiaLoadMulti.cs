using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;
using server.Hardware.GPU;
using server.Modules.Base;

namespace gpu_nvidia.Hardware.Sensors.Base
{
    public abstract class SensorBaseNVidiaLoadMulti : BaseGpuSensor, IMultiValueSensor
    {
        protected new NVidiaGpu Gpu;

        protected SensorBaseNVidiaLoadMulti(NVidiaGpu gpu)
            : base(gpu) {
            Gpu = gpu;
        }

        public override bool InitSensor() {
            return true;
        }

        public override BaseModule GetModuleHandle() {
            return ModuleGpuNvidia.ModuleHandle;
        }

        public abstract ISensorElement[] GetElements();
    }
}
