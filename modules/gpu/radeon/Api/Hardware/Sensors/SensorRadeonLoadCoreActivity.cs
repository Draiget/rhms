using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;
using server.Hardware;
using server.Utils;

namespace gpu_radeon.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorRadeonLoadCoreActivity : SensorBaseRadeonLoad
    {
        public SensorRadeonLoadCoreActivity(RadeonGpu gpu)
            : base(gpu) {
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            if (activity.Vddc > 0) {
                Value = Math.Min(activity.ActivityPercent, 100);

                if (!MinMaxSet) {
                    MinMaxSet = true;
                    MinValue = MaxValue = Value;
                } else {
                    MinValue = Math.Min(MinValue, Value);
                    MaxValue = Math.Max(MaxValue, Value);
                }

                IsSensorActive = true;
                return;
            }

            IsSensorActive = false;
        }

        public override string GetDisplayName() {
            return "Core Load";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }
}
