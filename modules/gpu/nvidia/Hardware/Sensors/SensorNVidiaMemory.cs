using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_nvidia.Api;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using gpu_nvidia.Hardware.Sensors.Base;
using server.Hardware;
using server.Utils;

namespace gpu_nvidia.Hardware.Sensors
{
    [SensorRegister]
    public class SensorNVidiaMemory : SensorBaseNVidiaLoadMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementNVidiaMemoryLoad _memAvail;
        private readonly SensorElementNVidiaMemoryLoad _memUsed;
        private readonly SensorElementNVidiaMemoryLoad _memFree;
        private readonly SensorElementNVidiaMemoryUsage _memLoad;

        public SensorNVidiaMemory(NVidiaGpu gpu)
            : base(gpu) {
            _memAvail = new SensorElementNVidiaMemoryLoad("total");
            _memUsed = new SensorElementNVidiaMemoryLoad("used");
            _memFree = new SensorElementNVidiaMemoryLoad("free");
            _memLoad = new SensorElementNVidiaMemoryUsage("load");

            _sensors = new ISensorElement[] {
                _memAvail, _memUsed, _memFree, _memLoad
            };
        }

        public override void Tick() {
            var info = ObrainMemoryInfo();
            if (info == null) {
                IsSensorActive = false;
                return;
            }

            var totalMem = info.Value.DedicatedVideoMemory;
            var freeMem = info.Value.CurAvailableDedicatedVideoMemory;
            var usedMem = (float)Math.Max(totalMem - freeMem, 0);

            _memFree.Update(freeMem / 1024f);
            _memAvail.Update(totalMem / 1024f);
            _memUsed.Update(usedMem / 1024);
            _memLoad.Update(100f * usedMem / totalMem);

            IsSensorActive = true;
        }

        public NvMemoryInfo? ObrainMemoryInfo() {
            var memoryInfo = new NvMemoryInfo {
                Version = NvApi.GpuMemoryInfoVer
            };

            if (NvApi.GetMemoryInfo(Gpu.AdapterHandle, ref memoryInfo) == NvStatus.Ok) {
                return memoryInfo;
            }

            return null;
        }

        public override SensorType GetSensorType() {
            return SensorType.MemoryLoad;
        }

        public override string GetDisplayName() {
            return "GPU Memory";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementNVidiaMemoryUsage : SensorElementNVidiaMemoryLoad
    {

        public SensorElementNVidiaMemoryUsage(string tag) 
            : base(tag) {
        }

        public override string GetMeasurement() {
            return "%";
        }
    }

    internal class SensorElementNVidiaMemoryLoad : SensorElementBaseNVidiaLoad<float>
    {
        private readonly string _tag;

        public SensorElementNVidiaMemoryLoad(string tag) {
            _tag = tag.ToLower();
        }

        public override void Update(float resultClock) {
            Value = resultClock;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "Mb";
        }

        public override string GetSystemTag() {
            return $"nvidia_memory_{_tag}";
        }
    }
}
