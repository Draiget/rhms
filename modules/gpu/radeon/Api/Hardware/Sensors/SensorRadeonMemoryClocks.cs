using System;
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
    public class SensorRadeonMemoryClocks : SensorBaseRadeonLoad
    {
        private readonly SensorElementRadeonMemoryClock _memoryClock;

        public SensorRadeonMemoryClocks(RadeonGpu gpu)
            : base(gpu) {
            _memoryClock = new SensorElementRadeonMemoryClock();
        }

        public override void TickSpecificLoad(AdlpmActivity activity) {
            _memoryClock.SetActive(activity.MemoryClock >= 0);
            if (activity.MemoryClock >= 0) {
                IsSensorActive = true;
                _memoryClock.Update(activity);
                return;
            }

            IsSensorActive = false;
        }

        public override ISensorElement GetElement() {
            return _memoryClock;
        }

        public override SensorType GetSensorType() {
            return SensorType.Clocks;
        }

        public override string GetDisplayName() {
            return "Memory Clock";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }
    }

    internal class SensorElementRadeonMemoryClock : SensorElementBaseRadeonLoad<AdlpmActivity>
    {
        public override void Update(AdlpmActivity info) {
            Value = 0.01f * info.MemoryClock;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "MHz";
        }

        public override string GetSystemTag() {
            return "radeon_clock_memory";
        }
    }
}
