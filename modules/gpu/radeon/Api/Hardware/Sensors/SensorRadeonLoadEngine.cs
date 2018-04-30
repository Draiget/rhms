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
    public class SensorRadeonLoadEngine : SensorBaseRadeonLoad
    {
        private readonly SensorElementRadeonLoadEngine _loadEngine;

        public SensorRadeonLoadEngine(RadeonGpu gpu)
            : base(gpu) {
            _loadEngine = new SensorElementRadeonLoadEngine();
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            _loadEngine.SetActive(activity.EngineClock >= 0);
            if (activity.EngineClock >= 0) {
                IsSensorActive = true;
                _loadEngine.SetActive(IsSensorActive);
                _loadEngine.Update(activity);
                return;
            }

            IsSensorActive = false;
            _loadEngine.SetActive(IsSensorActive);
        }

        public override ISensorElement GetElement() {
            return _loadEngine;
        }

        public override SensorType GetSensorType() {
            return SensorType.Utilization;
        }

        public override string GetDisplayName() {
            return "Engine Clock";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }

    internal class SensorElementRadeonLoadEngine : SensorElementBaseRadeonLoad<AdlpmActivity>
    {
        public override void Update(AdlpmActivity info) {
            Value = 0.01f * info.EngineClock;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "MHz";
        }

        public override string GetSystemTag() {
            return "radeon_clock_engine";
        }
    }
}
