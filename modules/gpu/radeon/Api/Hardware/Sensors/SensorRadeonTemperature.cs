using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Structures;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.GPU;
using server.Modules.Base;

namespace gpu_radeon.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorRadeonTemperature : BaseGpuSensor
    {
        private readonly RadeonGpu _gpu;
        private double _temperature;

        private bool _minMaxSet;
        private double _minValue;
        private double _maxValue;

        public SensorRadeonTemperature(RadeonGpu gpu)
            : base(gpu) {
            _gpu = gpu;
            _minMaxSet = false;
        }

        public override bool InitSensor() {
            _temperature = -1;
            return true;
        }

        public override double GetValue() {
            return _temperature;
        }

        public override double GetMin() {
            return _minValue;
        }

        public override double GetMax() {
            return _maxValue;
        }

        public override void Tick() {
            var temperature = new AdlTemperature();
            if (AdlApi.AdlGetAdapterTemperature(_gpu.AdapterIndex, 0, ref temperature) == AdlApi.AdlOk) {
                _temperature = 0.001f * temperature.Temperature;

                if (!_minMaxSet) {
                    _minMaxSet = true;
                    _minValue = _maxValue = _temperature;
                } else {
                    _minValue = Math.Min(_minValue, _temperature);
                    _maxValue = Math.Max(_maxValue, _temperature);
                }

                IsSensorActive = true;
            } else {
                IsSensorActive = false;
                _temperature = -1;
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
    }
}
