using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Hardware
{
    public abstract class GenericSensorHardware<T> where T : ISensorBase, IHardware
    {
        protected Dictionary<SensorType, List<T>> AvaliableSensors;

        protected GenericSensorHardware() {
            AvaliableSensors = new Dictionary<SensorType, List<T>>();
            foreach (SensorType type in Enum.GetValues(typeof(SensorType))) {
                AvaliableSensors[type] = new List<T>();
            }
        }

        public virtual void InitializeSensors() { }

        public bool IsSensorAvaliable(SensorType type) {
            return AvaliableSensors.ContainsKey(type);
        }

        protected void AddAvaliableSensor(T sensor) {
            AvaliableSensors[sensor.GetSensorType()].Add(sensor);
        }

        public T GetSensorByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return default(T);
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public T GetAllSensorsByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return default(T);
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public T[] GetAllSensors() {
            var list = new List<T>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s);
            }

            return list.ToArray();
        }

        public abstract HardwareIdentifer Identify();

        public T[] GetSensors() {
            var list = new List<T>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s);
            }

            return list.ToArray();
        }

        public abstract void TickUpdate();
    }
}
