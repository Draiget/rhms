using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using gpu_nvidia.Api;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using gpu_nvidia.Hardware.Sensors.Base;
using server.Hardware;
using server.Utils;

namespace gpu_nvidia.Hardware.Sensors
{
    [SensorRegister]
    public class SensorNVidiaTemperature : SensorBaseNVidiaLoadMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementNVidiaTemp[] _tempSensors;

        public SensorNVidiaTemperature(NVidiaGpu gpu)
            : base(gpu) 
        {
            IsSensorActive = NvApi.GetThermalSettings != null;
            if (NvApi.GetThermalSettings == null) {
                return;
            }

            var settings = ObtainThermalSettings();
            _tempSensors = new SensorElementNVidiaTemp[settings.Count];
            for (var i=0; i < _tempSensors.Length; i++) {
                _tempSensors[i] = new SensorElementNVidiaTemp(i, settings.Sensor[i]);
            }

            _sensors = new ISensorElement[_tempSensors.Length];
            for (var i=0; i < _tempSensors.Length; i++) {
                _sensors[i] = _tempSensors[i];
            }
        }

        public override void Tick() {
            var settings = ObtainThermalSettings();
            for (var i = 0; i < _tempSensors.Length; i++) {
                _tempSensors[i].Update(settings.Sensor[i]);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private NvGpuThermalSettings ObtainThermalSettings() {
            var settings = new NvGpuThermalSettings {
                Version = NvApi.GpuThermalSettingsVer,
                Count = NvApi.MaxThermalSensorsPerGpu
            };

            settings.Sensor = new NvSensor[settings.Count];
            if (NvApi.GetThermalSettings(Gpu.AdapterHandle, (int)NvThermalTarget.All, ref settings) != NvStatus.Ok) {
                settings.Count = 0;
            }

            return settings;
        }

        public override SensorType GetSensorType() {
            return SensorType.Temperature;
        }

        public override string GetDisplayName() {
            return "GPU Temperatures";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementNVidiaTemp : SensorElementBaseNVidiaLoad<NvSensor>
    {
        private NvSensor _nvidiaSensor;
        private readonly int _sensorIndex;
        private readonly string _tag;

        public SensorElementNVidiaTemp(int sensorIndex, NvSensor nvSensor) {
            _sensorIndex = sensorIndex;
            _nvidiaSensor = nvSensor;
            _tag = _nvidiaSensor.Target.ToString().ToLower();
        }

        public override void Update(NvSensor sensor) {
            _nvidiaSensor = sensor;
            Value = sensor.CurrentTemp;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "°C";
        }

        public override string GetSystemTag() {
            return $"nvidia_temp_{_tag}";
        }
    }
}