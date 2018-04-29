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
    public class SensorNVidiaClocks : SensorBaseNVidiaLoadMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementNVidiaClocks _clockCore;
        private readonly SensorElementNVidiaClocks _clockMemory;
        private readonly SensorElementNVidiaClocks _clockShader;

        public SensorNVidiaClocks(NVidiaGpu gpu)
            : base(gpu) 
        {
            IsSensorActive = NvApi.GetAllClocks != null;
            if (NvApi.GetAllClocks == null) {
                return;
            }

            _clockCore = new SensorElementNVidiaClocks("core");
            _clockMemory = new SensorElementNVidiaClocks("memory");
            _clockShader = new SensorElementNVidiaClocks("shader");

            _sensors = new ISensorElement[] {
                _clockCore, _clockMemory, _clockShader
            };
        }

        public override void Tick() {
            var clocks = ObtainClocks();
            if (clocks.Length >= 1) {
                _clockCore.SetActive(true);

                if (clocks.Length >= 29 && clocks[30] != 0) {
                    _clockCore.Update(0.0005f * clocks[30]);
                } else {
                    _clockCore.Update(0.001f * clocks[0]);
                }
            }

            if (clocks.Length >= 2) {
                _clockMemory.SetActive(true);
                _clockMemory.Update(0.001f * clocks[8]);
            }

            if (clocks.Length >= 3) {
                _clockShader.SetActive(true);

                if (clocks.Length >= 29 && clocks[30] != 0) {
                    _clockShader.Update(0.001f * clocks[30]);
                } else {
                    _clockShader.Update(0.001f * clocks[14]);
                }
            }
        }

        private uint[] ObtainClocks() {
            if (NvApi.GetAllClocks == null) {
                return null;
            }

            var allClocks = new NvClocks {
                Version = NvApi.GpuClocksVer,
                Clock = new uint[NvApi.MaxClocksPerGpu]
            };

            return NvApi.GetAllClocks(Gpu.AdapterHandle, ref allClocks) == NvStatus.Ok ? allClocks.Clock : null;
        }

        public override SensorType GetSensorType() {
            return SensorType.Clocks;
        }

        public override string GetDisplayName() {
            return "GPU Clocks";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementNVidiaClocks : SensorElementBaseNVidiaLoad<float>
    {
        private readonly string _tag;

        public SensorElementNVidiaClocks(string tag) {
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
