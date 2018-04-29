using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Hardware.Motherboard
{
    public abstract class BaseMotherboardSensor : ISensorBase
    {
        protected GenericMotherboard Motherboard;
        protected bool IsInitialized;
        protected bool IsSensorActive;

        protected BaseMotherboardSensor(GenericMotherboard motherboard) {
            Motherboard = motherboard;
        }

        public bool Initialize() {
            IsSensorActive = false;
            IsInitialized = InitSensor();
            return IsInitialized;
        }

        public abstract bool InitSensor();

        public bool IsAvaliable() {
            return Motherboard.IsSensorAvaliable(GetSensorType());
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
