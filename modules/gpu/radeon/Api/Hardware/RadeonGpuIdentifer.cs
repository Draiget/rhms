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
            HardwareRef = hardware;
        }

        public override HardwareType GetHardwareType(){
            return HardwareType.Gpu;
        }

        public override string GetVendor() {
            return "AMD";
        }

        public override string GetModel() {
            return HardwareRef.GetAdlInfo().AdapterName;
        }

        public override string GetFullSystemName() {
            return $"{HardwareRef.AdapterIndex}-" + HardwareRef.GetAdlInfo().AdapterName.Replace(' ', '_').ToLower();
        }

        public override string GetHardwareId() {
            return GetFullSystemName();
        }
    }
}
