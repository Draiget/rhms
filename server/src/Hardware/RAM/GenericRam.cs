using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace server.Hardware.RAM
{
    public abstract class GenericRam : IHardware
    {
        protected Dictionary<SensorType, List<BaseRamSensor>> AvaliableSensors;

        protected GenericRam() {
            AvaliableSensors = new Dictionary<SensorType, List<BaseRamSensor>>();
            foreach (SensorType type in Enum.GetValues(typeof(SensorType))) {
                AvaliableSensors[type] = new List<BaseRamSensor>();
            }
        }

        public virtual void InitializeSensors() { }

        public bool IsSensorAvaliable(SensorType type) {
            return AvaliableSensors.ContainsKey(type);
        }

        protected void AddAvaliableSensor(BaseRamSensor sensor) {
            AvaliableSensors[sensor.GetSensorType()].Add(sensor);
        }

        public BaseRamSensor GetSensorByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return null;
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public BaseRamSensor GetAllSensorsByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return null;
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public BaseRamSensor[] GetAllSensors() {
            var list = new List<BaseRamSensor>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s);
            }

            return list.ToArray();
        }

        public abstract HardwareIdentifer Identify();

        public ISensorBase[] GetSensors() {
            var list = new List<ISensorBase>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s);
            }

            return list.ToArray();
        }

        public abstract void TickUpdate();
    }
}
