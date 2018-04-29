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
    //[SensorRegister]
    public class SensorNVidiaMemory : SensorBaseNVidiaLoadMulti
    {
        private readonly ISensorElement[] _sensors;

        public SensorNVidiaMemory(NVidiaGpu gpu)
            : base(gpu) {
        }

        public override void Tick() {
            throw new NotImplementedException();
        }

        public NvMemoryInfo? ObrainMemoryInfo() {
            var memoryInfo = new NvMemoryInfo {
                Version = NvApi.GpuMemoryInfoVer,
                Values = new uint[NvApi.MaxMemoryValuesPerGpu]
            };

            /*if (NvApi.GetMemoryInfo(Gpu.AdapterHandle, ref memoryInfo) == NvStatus.Ok) {
                return memoryInfo;
            }*/

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
            return "MHz";
        }

        public override string GetSystemTag() {
            return $"clock_{_tag}";
        }
    }
}
