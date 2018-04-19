using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Hardware.GPU
{
    public abstract class BaseGpuSensor : ISensor
    {
        protected readonly GenericGpu Gpu;
        protected bool IsInitialized;
        protected bool IsSensorActive;

        protected BaseGpuSensor(GenericGpu gpu){
            Gpu = gpu;
        }

        public bool Initialize() {
            IsSensorActive = false;
            IsInitialized = InitSensor();
            return IsInitialized;
        }

        public abstract bool InitSensor();

        public bool IsAvaliable(){
            return Gpu.IsSensorAvaliable(GetSensorType());
        }

        public bool IsActive() {
            return IsSensorActive;
        }

        public virtual double GetMax() {
            return double.NaN;
        }

        public virtual double GetMin(){
            return double.NaN;
        }

        public abstract double GetValue();

        public abstract void Tick();

        public abstract SensorType GetSensorType();

        public abstract string GetDisplayName();

        public abstract string GetSystemName();

        public abstract BaseModule GetModuleHandle();
    }
}
