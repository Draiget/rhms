using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using base_motherboards.Api.Hardware.Sensors.Base;
using server.Drivers.Presentation;
using server.Hardware;
using server.Utils;
using server.Utils.Natives;

namespace base_motherboards.Api.Hardware.Sensors
{
    [SensorRegister]
    public class RamLoadSensor : SensorBaseRamMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementRamUsage _usedMemory;
        private readonly SensorElementRamUsage _memoryTotal;
        private readonly SensorElementRamUsage _memoryAvail;
        private readonly SensorElementRamLoad _memoryLoad;

        public RamLoadSensor(AnyRam ram)
            : base(ram) 
        {
            _usedMemory = new SensorElementRamUsage("used");
            _memoryTotal = new SensorElementRamUsage("total");
            _memoryLoad = new SensorElementRamLoad("load");
            _memoryAvail = new SensorElementRamLoad("available");

            _sensors = new ISensorElement[] {
                _memoryTotal, _usedMemory, _memoryAvail, _memoryLoad
            };
        }

        public override SensorType GetSensorType() {
            return SensorType.MemoryLoad;
        }

        public override string GetDisplayName() {
            return "Generic Memory Load";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override void TickSpecificLoad(MemoryStatusEx? info) {
            if (info == null) {
                IsSensorActive = false;
                return;
            }

            _memoryTotal.SetActive(IsSensorActive);
            _memoryTotal.Update((float)info.Value.TotalPhysicalMemory / (1024 * 1024 * 1024));
            _memoryLoad.SetActive(IsSensorActive);
            _memoryLoad.Update(100.0f - (100.0f * info.Value.AvailablePhysicalMemory) / info.Value.TotalPhysicalMemory);
            _usedMemory.SetActive(IsSensorActive);
            _usedMemory.Update((float)(info.Value.TotalPhysicalMemory - info.Value.AvailablePhysicalMemory) / (1024 * 1024 * 1024));
            _memoryAvail.SetActive(IsSensorActive);
            _memoryAvail.Update((float)info.Value.AvailablePhysicalMemory / (1024 * 1024 * 1024));

            IsSensorActive = true;
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementRamLoad : SensorElementRamUsage
    {
        public SensorElementRamLoad(string tag)
            : base(tag) {
        }

        public override string GetMeasurement() {
            return "%";
        }
    }

    internal class SensorElementRamUsage : SensorElementBaseRam<float>
    {
        private readonly string _tag;

        public SensorElementRamUsage(string tag) {
            _tag = tag;
        }

        public override void Update(float load) {
            Value = load;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "Gb";
        }

        public override string GetSystemTag() {
            return $"ram_{_tag}";
        }
    }
}
