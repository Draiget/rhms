using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;

namespace gpu_radeon.Api.Hardware.Sensors
{
    public class SensorRadeonLoadEngine : SensorBaseRadeonLoad
    {
        public SensorRadeonLoadEngine(RadeonGpu gpu)
            : base(gpu) {
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            if (activity.EngineClock > 0) {
                Value = 0.01f * activity.EngineClock;
                IsSensorActive = true;
                return;
            }

            IsSensorActive = false;
        }

        public override string GetDisplayName() {
            return "Engine Clock";
        }

        public override string GetSystemName() {
            return "engine_clock";
        }
    }
}
