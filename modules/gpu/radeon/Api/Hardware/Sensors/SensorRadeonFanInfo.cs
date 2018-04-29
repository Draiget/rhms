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
    public class SensorRadeonFanInfo : SensorBaseRadeonLoadMulti
    {
        private readonly SensorElementRadeonFanSpeedMinRpm _rpmMinSensor;
        private readonly SensorElementRadeonFanSpeedMaxRpm _rpmMaxSensor;
        private readonly SensorElementRadeonFanSpeedCurrentRpm _rpmCurrentSensor;

        private readonly ISensorElement[] _sensors;

        public SensorRadeonFanInfo(RadeonGpu gpu)
            : base(gpu) 
        {
            _rpmMinSensor = new SensorElementRadeonFanSpeedMinRpm();
            _rpmMaxSensor = new SensorElementRadeonFanSpeedMaxRpm();
            _rpmCurrentSensor = new SensorElementRadeonFanSpeedCurrentRpm();

            _sensors = new ISensorElement[] {
                _rpmCurrentSensor, _rpmMinSensor, _rpmMaxSensor
            };
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            var adlSpeed = new AdlFanSpeedValue();
            if (AdlApi.AdlGetFanSpeed(Gpu.AdapterIndex, 0, ref adlSpeed) == AdlApi.AdlOk) {
                _rpmCurrentSensor.Update(adlSpeed);
                _rpmCurrentSensor.SetActive(true);
            } else {
                _rpmCurrentSensor.SetActive(false);
            }

            var adlSpeedInfo = new AdlFanSpeedInfo();
            if (AdlApi.AdlGetFanSpeedInfo(Gpu.AdapterIndex, 0, ref adlSpeedInfo) != AdlApi.AdlOk) {
                _rpmMaxSensor.Update(adlSpeedInfo);
                _rpmMinSensor.Update(adlSpeedInfo);
                _rpmMaxSensor.SetActive(true);
                _rpmMinSensor.SetActive(true);
            } else {
                _rpmMaxSensor.SetActive(false);
                _rpmMinSensor.SetActive(false);
            }

            IsSensorActive = true;
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }

        public override SensorType GetSensorType() {
            return SensorType.FanSpd;
        }

        public override string GetDisplayName() {
            return "FAN Info";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }

    internal class SensorElementRadeonFanSpeedMaxRpm : SensorElementBaseRadeonLoad<AdlFanSpeedInfo>
    {
        public override void Update(AdlFanSpeedInfo info) {
            Value = info.MaxRPM;
            AfterUpdate();
        }

        public override string GetSystemTag() {
            return "max_rpm";
        }
    }

    internal class SensorElementRadeonFanSpeedMinRpm : SensorElementBaseRadeonLoad<AdlFanSpeedInfo>
    {
        public override void Update(AdlFanSpeedInfo info) {
            Value = info.MinRPM;
            AfterUpdate();
        }

        public override string GetSystemTag() {
            return "min_rpm";
        }
    }

    internal class SensorElementRadeonFanSpeedCurrentRpm : SensorElementBaseRadeonLoad<AdlFanSpeedValue>
    {
        public override void Update(AdlFanSpeedValue info) {
            Value = info.FanSpeed;
            AfterUpdate();
        }

        public override string GetSystemTag() {
            return "fan_rpm";
        }
    }
}
