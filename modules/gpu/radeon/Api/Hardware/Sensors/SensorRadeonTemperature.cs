using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;
using server.Hardware.GPU;

namespace gpu_radeon.Api.Hardware.Sensors
{
    public class SensorRadeonTemperature : BaseGpuSensor
    {
        public SensorRadeonTemperature(GenericGpu gpu)
            : base(gpu){
        }

        public override SensorType GetSensorType(){
            return SensorType.Temperature;
        }
    }
}
