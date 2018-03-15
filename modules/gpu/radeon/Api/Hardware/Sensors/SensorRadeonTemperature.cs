using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Structures;
using server.Hardware;
using server.Hardware.GPU;

namespace gpu_radeon.Api.Hardware.Sensors
{
    public class SensorRadeonTemperature : BaseGpuSensor
    {
        private readonly RadeonGpu _gpu;
        private double _temperature;

        public SensorRadeonTemperature(RadeonGpu gpu)
            : base(gpu) {
            _gpu = gpu;
        }

        public override bool InitSensor() {
            _temperature = -1;
            return true;
        }

        public override double GetValue() {
            return _temperature;
        }

        public override void Tick() {
            var temperature = new AdlTemperature();
            if (AdlApi.AdlGetAdapterTemperature(_gpu.AdapterIndex, 0, ref temperature) == AdlApi.AdlOk) {
                _temperature = 0.001f * temperature.Temperature;
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
    }
}
