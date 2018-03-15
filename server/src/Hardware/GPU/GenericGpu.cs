using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;

namespace server.Hardware.GPU
{
    public abstract class GenericGpu : IHardware
    {
        protected Dictionary<SensorType,List<BaseGpuSensor>> AvaliableSensors;

        protected GenericGpu(){
            AvaliableSensors = new Dictionary<SensorType, List<BaseGpuSensor>>();
            foreach (SensorType type in Enum.GetValues(typeof(SensorType))) {
                AvaliableSensors[type] = new List<BaseGpuSensor>();
            }
        }

        public abstract void InitializeInformation();

        public bool IsSensorAvaliable(SensorType type){
            return AvaliableSensors.ContainsKey(type);
        }

        protected void AddAvaliableSensor(SensorType sensorType, BaseGpuSensor sensor){
            AvaliableSensors[sensorType].Add(sensor);
        }

        public BaseGpuSensor GetSensorByType(SensorType type){
            if (!IsSensorAvaliable(type)) {
                return null;
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public BaseGpuSensor GetAllSensorsByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return null;
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public BaseGpuSensor[] GetAllSensors(){
            var list = new List<BaseGpuSensor>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s);
            }

            return list.ToArray();
        }

        public abstract HardwareIdentifer Identify();

        public ISensor[] GetSensors(){
            var list = new List<ISensor>();
            foreach (var s in AvaliableSensors.Values) {
                list.AddRange(s);
            }

            return list.ToArray();
        }

        public abstract void TickUpdate();
    }
}
