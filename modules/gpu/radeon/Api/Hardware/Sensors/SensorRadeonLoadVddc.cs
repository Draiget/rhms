using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;

namespace gpu_radeon.Api.Hardware.Sensors
{
    public class SensorRadeonLoadVddc : SensorBaseRadeonLoad
    {
        public SensorRadeonLoadVddc(RadeonGpu gpu)
            : base(gpu) {
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            if (activity.Vddc > 0) {
                Value = 0.001f * activity.Vddc;
                IsSensorActive = true;
                return;
            }

            IsSensorActive = false;
        }

        public override string GetDisplayName() {
            return "VDDC";
        }

        public override string GetSystemName() {
            return GetDisplayName().ToLower();
        }
    }
}
