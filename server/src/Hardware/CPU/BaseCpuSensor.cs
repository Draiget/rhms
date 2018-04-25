using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Hardware.CPU
{
    public abstract class BaseCpuSensor : ISensorBase
    {
        protected GenericCpu Cpu;
        protected bool IsInitialized;
        protected bool IsSensorActive;

        protected BaseCpuSensor(GenericCpu cpu) {
            Cpu = cpu;
        }

        public bool Initialize() {
            IsSensorActive = false;
            IsInitialized = InitSensor();
            return IsInitialized;
        }

        public abstract bool InitSensor();

        public bool IsAvaliable() {
            return Cpu.IsSensorAvaliable(GetSensorType());
        }

        public bool IsActive() {
            return IsSensorActive;
        }

        public abstract void Tick();

        public abstract SensorType GetSensorType();

        public abstract string GetDisplayName();

        public abstract string GetSystemName();

        public abstract BaseModule GetModuleHandle();
    }
}
