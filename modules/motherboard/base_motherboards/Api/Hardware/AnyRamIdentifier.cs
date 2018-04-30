using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace base_motherboards.Api.Hardware
{
    public class AnyRamIdentifier : HardwareIdentifer
    {
        public AnyRam Ram;

        public AnyRamIdentifier(AnyRam hardware)
            : base(hardware) {

        }

        public override HardwareType GetHardwareType() {
            return HardwareType.Ram;
        }

        public override string GetVendor() {
            return "internal";
        }

        public override string GetModel() {
            return "internal";
        }

        public override string GetFullSystemName() {
            return "Shared Memory Info";
        }

        public override string GetHardwareId() {
            return "shared";
        }
    }
}
