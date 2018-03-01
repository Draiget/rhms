using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Hardware.GPU
{
    public abstract class BaseGpuSensor : ISensor
    {
        private readonly GenericGpu _gpu;

        protected BaseGpuSensor(GenericGpu gpu){
            _gpu = gpu;
        }

        public bool IsAvaliable(){
            return _gpu.IsSensorAvaliable(GetSensorType());
        }

        public double GetMax(){
            throw new NotImplementedException();
        }

        public double GetMin(){
            throw new NotImplementedException();
        }

        public double GetValue(){
            throw new NotImplementedException();
        }

        public abstract SensorType GetSensorType();
    }
}
