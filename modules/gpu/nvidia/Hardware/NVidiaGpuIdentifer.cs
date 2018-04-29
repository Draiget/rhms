using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace gpu_nvidia.Hardware
{
    public class NVidiaGpuIdentifer : HardwareIdentifer
    {
        protected new NVidiaGpu HardwareRef;

        public NVidiaGpuIdentifer(NVidiaGpu hardware)
            : base(hardware) {
            HardwareRef = hardware;
        }

        public override HardwareType GetHardwareType() {
            return HardwareType.Gpu;
        }

        public override string GetVendor() {
            return "NVidia";
        }

        public override string GetModel() {
            return HardwareRef.GpuModelName;
        }

        public override string GetFullSystemName() {
            return HardwareRef.GpuFullName;
        }
    }
}
