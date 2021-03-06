﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors;
using gpu_radeon.Api.Structures;
using server.Hardware;
using server.Hardware.GPU;
using server.Utils;

namespace gpu_radeon.Api.Hardware
{
    public class RadeonGpu : GenericGpu
    {
        private readonly RadeonGpuIdentifer _hardwareIdentifer;
        private readonly AdlAdapterInfo _adapterInfo;

        public RadeonGpu(AdlAdapterInfo info) {
            _adapterInfo = info;
            _hardwareIdentifer = new RadeonGpuIdentifer(this);
        }

        public int AdapterIndex => _adapterInfo.AdapterIndex;

        public bool IsActive {
            get;
            private set;
        }

        public AdlAdapterInfo GetAdlInfo() {
            return _adapterInfo;
        }

        public int AdapterId {
            get;
            private set;
        }

        public override void InitializeSensors(){
            var sensors = AssemblyUtils.GetAttributesInModule(typeof(RadeonGpu), typeof(SensorRegisterAttribute));
            foreach (var sensor in sensors) {
                AddAvaliableSensor((BaseGpuSensor)Activator.CreateInstance(sensor, this));
            }
        }

        public override HardwareIdentifer Identify(){
            return _hardwareIdentifer;
        }

        public override void TickUpdate() {
            var sensors = GetSensors();
            foreach (var sensor in sensors) {
                if (sensor.IsAvaliable()) {
                    sensor.Tick();
                }
            }
        }

        public void SetActive(bool status) {
            IsActive = status;
        }

        public void SetAdapterId(int adapterId) {
            AdapterId = adapterId;
        }

        public bool IsSameDeviceAs(AdlAdapterInfo info) {
            return info.DeviceNumber == _adapterInfo.DeviceNumber &&
                   info.BusNumber == _adapterInfo.BusNumber;
        }

        public override string ToString() {
            return $"RadeonGpu[Name='{_adapterInfo.AdapterName}', DeviceNumber={_adapterInfo.DeviceNumber}, BusNumber={_adapterInfo.BusNumber}]";
        }
    }
}
