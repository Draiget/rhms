using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;

namespace gpu_radeon.Api.Hardware.Sensors
{
    public class SensorRadeonLoadMemory : SensorBaseRadeonLoad
    {
        public SensorRadeonLoadMemory(RadeonGpu gpu)
            : base(gpu) {
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            if (activity.MemoryClock > 0) {
                Value = 0.01f * activity.MemoryClock;

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
            return "Memory Clock";
        }

        public override string GetSystemName() {
            return "mem_clock";
        }
    }
}
