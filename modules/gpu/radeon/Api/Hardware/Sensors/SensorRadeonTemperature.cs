using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.GPU;
using server.Modules.Base;

namespace gpu_radeon.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorRadeonTemperature : BaseGpuSensor, ISingleValueSensor
    {
        protected new RadeonGpu Gpu;
        private readonly SensorElementRadeonTemperature _sensorTemperature;

        public SensorRadeonTemperature(RadeonGpu gpu)
            : base(gpu) {
            Gpu = gpu;
            _sensorTemperature = new SensorElementRadeonTemperature();
        }

        public override bool InitSensor() {
            _sensorTemperature.SetValue(-1);
            return true;
        }

        public override void Tick() {
            var temperature = new AdlTemperature();
            if (AdlApi.AdlGetAdapterTemperature(Gpu.AdapterIndex, 0, ref temperature) == AdlApi.AdlOk) {
                _sensorTemperature.Update(temperature);
                IsSensorActive = true;
                _sensorTemperature.SetActive(IsSensorActive);
            } else {
                IsSensorActive = false;
                _sensorTemperature.SetActive(IsSensorActive);
            }
        }

        public override SensorType GetSensorType(){
            return SensorType.Temperature;
        }

        public override string GetDisplayName() {
            return "Temperature";
        }

        public override string GetSystemName() {
            return "temp";
        }

        public override BaseModule GetModuleHandle() {
            return ModuleGpuRadeon.ModuleHandle;
        }

        public ISensorElement GetElement() {
            return _sensorTemperature;
        }
    }

    internal class SensorElementRadeonTemperature : SensorElementBaseRadeonLoad<AdlTemperature>
    {
        public override void Update(AdlTemperature info) {
            Value = 0.001f * info.Temperature;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "°C";
        }

        public void SetValue(double value) {
            Value = value;
        }

        public override string GetSystemTag() {
            return "temp";
        }
    }
}
