using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Hardware.GPU
{
    public class GpuSensorThermal : BaseGpuSensor
    {
        public GpuSensorThermal(GenericGpu gpu)
            : base(gpu){
        }

        public override SensorType GetSensorType(){
            return SensorType.Temperature;
        }
    }
}
