using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace gpu_radeon.Api.Hardware
{
    public class RadeonGpuIdentifer : HardwareIdentifer
    {
        protected new RadeonGpu HardwareRef;

        public RadeonGpuIdentifer(RadeonGpu hardware)
            : base(hardware) {
        }

        public override HardwareType GetHardwareType(){
            return HardwareType.Gpu;
        }

        public override string GetVendor(){
            throw new NotImplementedException();
        }

        public override string GetModel(){
            throw new NotImplementedException();
        }
    }
}
