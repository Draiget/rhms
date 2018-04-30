using System;
using System.Globalization;
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

        public string DeviceId {
            get;
        }

        public string SubSystemId {
            get;
        }

        public string RevisionId {
            get;
        }

        public uint BusId => _busId;

        private readonly uint _busId;
        private readonly NVidiaGpuIdentifer _gpuIdentifer;

        public NVidiaGpu(int adapterIndex, NvPhysicalGpuHandle handle) {
            AdapterHandle = handle;
            _gpuIdentifer = new NVidiaGpuIdentifer(this);

            GpuModelName = NvApi.NvApiGpuGetFullName(handle, out var gpuName) == NvStatus.Ok ? gpuName.Trim() : "Unknown";

            if (NvApi.GetPciIdentifiers != null && NvApi.GetPciIdentifiers(AdapterHandle, out var deviceId, out var subSystemId, out var revisionId, out var extDeviceId) == NvStatus.Ok) {
                DeviceId = $"0x{deviceId:X}";
                SubSystemId = $"0x{subSystemId:X}";
                RevisionId = $"0x{revisionId:X}";
            }

            if (NvApi.GetBusSlotId != null && NvApi.GetBusId(AdapterHandle, out _busId) != NvStatus.Ok) {
                throw new Exception("Unable to obtain bus slot id");
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
