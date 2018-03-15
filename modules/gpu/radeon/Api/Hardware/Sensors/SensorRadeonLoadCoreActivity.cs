using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;

namespace gpu_radeon.Api.Hardware.Sensors
{
    public class SensorRadeonLoadCoreActivity : SensorBaseRadeonLoad
    {
        public SensorRadeonLoadCoreActivity(RadeonGpu gpu)
            : base(gpu) {
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            if (activity.Vddc > 0) {
                Value = Math.Min(activity.ActivityPercent, 100);
                IsSensorActive = true;
                return;
            }

            IsSensorActive = false;
        }

        public override string GetDisplayName() {
            return "Core Load";
        }

        public override string GetSystemName() {
            return "core_load";
        }
    }
}
