﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Hardware.Sensors.Base;
using gpu_radeon.Api.Structures;
using server.Drivers.Presentation;
using server.Hardware;
using server.Utils;

namespace gpu_radeon.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorRadeonLoadMemory : SensorBaseRadeonLoad
    {
        private readonly SensorElementRadeonLoadMemory _loadMemory;

        public SensorRadeonLoadMemory(RadeonGpu gpu)
            : base(gpu) {
            _loadMemory = new SensorElementRadeonLoadMemory();
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            _loadMemory.SetActive(activity.MemoryClock >= 0);
            if (activity.MemoryClock >= 0) {
                IsSensorActive = true;
                _loadMemory.Update(activity);
                return;
            }

            IsSensorActive = false;
        }

        public override ISensorElement GetElement() {
            return _loadMemory;
        }

        public override string GetDisplayName() {
            return "Memory Clock";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }

    internal class SensorElementRadeonLoadMemory : SensorElementBaseRadeonLoad<AdlpmActivity>
    {
        public override void Update(AdlpmActivity info) {
            Value = 0.01f * info.MemoryClock;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "MHz";
        }

        public override string GetSystemTag() {
            return "load_mem_mhz";
        }
    }
}
