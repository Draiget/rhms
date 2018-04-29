using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware.CPU;

namespace server.Hardware
{
    public abstract class BaseSensorsHarwdare<T> where T : ISensorBase
    {
        protected Dictionary<SensorType, List<T>> AvaliableSensors;

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

        public ISensorBase[] GetSensors() {
            var list = new List<ISensorBase>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s.Cast<ISensorBase>());
            }

            return list.ToArray();
        }

        public abstract void TickUpdate();
    }
}
