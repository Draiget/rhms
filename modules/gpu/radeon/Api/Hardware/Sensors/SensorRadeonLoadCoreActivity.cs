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
        private readonly SensorElementRadeonLoadCoreActivity _coreActivity;

        public SensorRadeonLoadCoreActivity(RadeonGpu gpu)
            : base(gpu) {
            _coreActivity = new SensorElementRadeonLoadCoreActivity();
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            _coreActivity.SetActive(activity.ActivityPercent >= 0);
            if (activity.ActivityPercent >= 0) {
                IsSensorActive = true;
                _coreActivity.Update(activity);
                return;
            }

            IsSensorActive = false;
        }

        public override ISensorElement GetElement() {
            return _coreActivity;
        }

        public override string GetDisplayName() {
            return "Core Load";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }

    internal class SensorElementRadeonLoadCoreActivity : SensorElementBaseRadeonLoad<AdlpmActivity>
    {
        public override void Update(AdlpmActivity info) {
            Value = Math.Min(info.ActivityPercent, 100);
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "%";
        }

        public override string GetSystemTag() {
            return "load_percent";
        }
    }
}
