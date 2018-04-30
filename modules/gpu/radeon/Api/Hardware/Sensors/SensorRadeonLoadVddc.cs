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
    public class SensorRadeonLoadVddc : SensorBaseRadeonLoad
    {
        private readonly SensorElementRadeonVddc _sensorVddc;

        public SensorRadeonLoadVddc(RadeonGpu gpu)
            : base(gpu) {
            _sensorVddc = new SensorElementRadeonVddc();
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            _sensorVddc.SetActive(activity.Vddc >= 0);
            if (activity.Vddc >= 0) {
                IsSensorActive = true;
                _sensorVddc.SetActive(IsSensorActive);
                _sensorVddc.Update(activity);
                return;
            }

            IsSensorActive = false;
            _sensorVddc.SetActive(IsSensorActive);
        }

        public override ISensorElement GetElement() {
            return _sensorVddc;
        }

        public override SensorType GetSensorType() {
            return SensorType.Voltage;
        }

        public override string GetDisplayName() {
            return "VDDC";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }

    internal class SensorElementRadeonVddc : SensorElementBaseRadeonLoad<AdlpmActivity>
    {
        public override void Update(AdlpmActivity info) {
            Value = 0.001f * info.Vddc;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "V";
        }

        public override string GetSystemTag() {
            return "radeon_core_vddc";
        }
    }
}
