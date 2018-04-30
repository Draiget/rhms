using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Hardware.RAM
{
    public abstract class BaseRamSensor : ISensorBase
    {
        protected GenericRam Ram;
        protected bool IsInitialized;
        protected bool IsSensorActive;

        protected BaseRamSensor(GenericRam ram) {
            Ram = ram;
        }

        public bool Initialize() {
            IsSensorActive = false;
            IsInitialized = InitSensor();
            return IsInitialized;
        }

        public abstract bool InitSensor();

        public bool IsAvaliable() {
            return Ram.IsSensorAvaliable(GetSensorType());
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
