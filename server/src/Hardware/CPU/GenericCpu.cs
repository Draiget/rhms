﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;

namespace server.Hardware.CPU
{
    public abstract class GenericCpu : IHardware
    {
        protected Dictionary<SensorType, List<BaseCpuSensor>> AvaliableSensors;

        protected readonly CpuidProcessorInfo ProcessorCpuid;

        public CpuVendor Vendor => ProcessorCpuid[0, 0].Vendor;

        protected GenericCpu(CpuidProcessorInfo cpuInfo){
            ProcessorCpuid = cpuInfo;

            AvaliableSensors = new Dictionary<SensorType, List<BaseCpuSensor>>();
            foreach (SensorType type in Enum.GetValues(typeof(SensorType))) {
                AvaliableSensors[type] = new List<BaseCpuSensor>();
            }
        }

        public CpuidProcessorInfo GetCpuidProcessorInfo() {
            return ProcessorCpuid;
        }

        public virtual void InitializeSensors() { }

        public bool IsSensorAvaliable(SensorType type) {
            return AvaliableSensors.ContainsKey(type);
        }

        protected void AddAvaliableSensor(BaseCpuSensor sensor) {
            AvaliableSensors[sensor.GetSensorType()].Add(sensor);
        }

        public BaseCpuSensor GetSensorByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return null;
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public BaseCpuSensor GetAllSensorsByType(SensorType type) {
            if (!IsSensorAvaliable(type)) {
                return null;
            }

            return AvaliableSensors[type].FirstOrDefault();
        }

        public BaseCpuSensor[] GetAllSensors() {
            var list = new List<BaseCpuSensor>();
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
