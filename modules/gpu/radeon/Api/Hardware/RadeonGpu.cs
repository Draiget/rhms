using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors;
using server.Hardware;
using server.Hardware.GPU;

namespace gpu_radeon.Api.Hardware
{
    public class RadeonGpu : GenericGpu
    {
        private readonly RadeonGpuIdentifer _hardwareIdentifer;

        public RadeonGpu(){
            AddAvaliableSensor(SensorType.Temperature, new SensorRadeonTemperature(this));
            _hardwareIdentifer = new RadeonGpuIdentifer(this);
        }

        public override void InitializeInformation(){

        }

        public override HardwareIdentifer Identify(){
            return _hardwareIdentifer;
        }
    }
}
