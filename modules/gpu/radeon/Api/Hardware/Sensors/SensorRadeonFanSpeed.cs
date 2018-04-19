using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;
using server.Hardware;
using server.Hardware.GPU;
using server.Modules.Base;
using server.Utils;

namespace gpu_radeon.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorRadeonFanSpeed : SensorBaseRadeonLoad
    {
        public SensorRadeonFanSpeed(RadeonGpu gpu)
            : base(gpu) {
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            var adlSpeed = new AdlFanSpeedValue();
            if (AdlApi.AdlGetFanSpeed(Gpu.AdapterIndex, 0, ref adlSpeed) != AdlApi.AdlOk) {
                IsSensorActive = false;
                return;
            }

            Value = adlSpeed.FanSpeed;
            IsSensorActive = true;
        }

        public override string GetDisplayName() {
            return "FAN Speed";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }
}
