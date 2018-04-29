using System;
using gpu_nvidia.Api;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using server.Hardware;
using server.Hardware.GPU;
using server.Utils;

namespace gpu_nvidia.Hardware
{
    public class NVidiaGpu : GenericGpu
    {
        public NvPhysicalGpuHandle AdapterHandle {
            get;
            private set;
        }

        public string GpuFullName {
            get;
        }

        public string GpuModelName {
            get;
        }

        private readonly NVidiaGpuIdentifer _gpuIdentifer;

        public NVidiaGpu(int adapterIndex, NvPhysicalGpuHandle handle) {
            AdapterHandle = handle;
            _gpuIdentifer = new NVidiaGpuIdentifer(this);

            if (NvApi.NvApiGpuGetFullName(handle, out var gpuName) == NvStatus.Ok) {
                GpuModelName = gpuName.Trim();
            } else {
                GpuModelName = "Unknown";
            }

            GpuFullName = $"NVIDIA {GpuModelName}";
        }

        public bool IsActive {
            get;
            private set;
        }

        public override void InitializeSensors() {
            var sensors = AssemblyUtils.GetAttributesInModule(typeof(NVidiaGpu), typeof(SensorRegisterAttribute));
            foreach (var sensor in sensors) {
                AddAvaliableSensor((BaseGpuSensor)Activator.CreateInstance(sensor, this));
            }
        }

        public override HardwareIdentifer Identify() {
            return _gpuIdentifer;
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

        public override string ToString() {
            return $"NVidiaGpu [Name={GpuFullName}, Handle={AdapterHandle}]";
        }
    }
}
