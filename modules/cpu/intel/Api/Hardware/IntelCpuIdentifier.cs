using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace cpu_intel.Api.Hardware
{
    public class IntelCpuIdentifier : HardwareIdentifer
    {
        protected new IntelCpu HardwareRef;

        public IntelCpuIdentifier(IntelCpu hardware)
            : base(hardware) {
            HardwareRef = hardware;
        }

        public override HardwareType GetHardwareType() {
            return HardwareType.Cpu;
        }

        public override string GetVendor() {
            return HardwareRef.Vendor.ToString();
        }

        public override string GetModel() {
            return HardwareRef.ModelName;
        }

        public override string GetFullSystemName() {
            return HardwareRef.FullName;
        }

        public override string GetHardwareId() {
            return $"{HardwareRef.ProcessorId}-{GetModel().Replace(' ', '_').ToLower().Trim()}";
        }
    }
}
