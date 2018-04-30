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
    public class SensorNVidiaLoads : SensorBaseNVidiaLoadMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementNVidiaLoad _loadCore;
        private readonly SensorElementNVidiaLoad _loadFrameBuffer;
        private readonly SensorElementNVidiaLoad _loadVideoEngine;
        private readonly SensorElementNVidiaLoad _loadBus;

        public SensorNVidiaLoads(NVidiaGpu gpu)
            : base(gpu) 
        {
            _loadCore = new SensorElementNVidiaLoad("core");
            _loadFrameBuffer = new SensorElementNVidiaLoad("frame_buffer");
            _loadVideoEngine = new SensorElementNVidiaLoad("video_engine");
            _loadBus = new SensorElementNVidiaLoad("bus_interface");

            _sensors = new ISensorElement[] {
                _loadCore, _loadFrameBuffer, _loadVideoEngine, _loadBus
            };

            var states = ObtainPStates();
        }

        public override void Tick() {
            var states = ObtainPStates();
            if (states == null) {
                IsSensorActive = false;
                return;
            }

            _loadCore.SetActive(states.Value.PStates[0].Present);
            _loadCore.Update(states.Value.PStates[0].Percentage);

            _loadFrameBuffer.SetActive(states.Value.PStates[1].Present);
            _loadFrameBuffer.Update(states.Value.PStates[1].Percentage);

            _loadVideoEngine.SetActive(states.Value.PStates[2].Present);
            _loadVideoEngine.Update(states.Value.PStates[2].Percentage);

            _loadBus.SetActive(states.Value.PStates[3].Present);
            _loadBus.Update(states.Value.PStates[3].Percentage);

            IsSensorActive = true;
        }

        public NvPStates? ObtainPStates() {
            var states = new NvPStates {
                Version = NvApi.GpuPstatesVer,
                PStates = new NvPState[NvApi.MaxPstatesPerGpu]
            };

            if (NvApi.GetDynamicPStatesInfoEx == null || NvApi.GetDynamicPStatesInfoEx(Gpu.AdapterHandle, ref states) != NvStatus.Ok) {
                return null;
            }

            return states;
        }

        public override SensorType GetSensorType() {
            return SensorType.MemoryLoad;
        }

        public override string GetDisplayName() {
            return "GPU Loads";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementNVidiaLoad : SensorElementBaseNVidiaLoad<float>
    {
        private readonly string _tag;

        public SensorElementNVidiaLoad(string tag) {
            _tag = tag.ToLower();
        }

        public override void Update(float resultClock) {
            Value = resultClock;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "%";
        }

        public override string GetSystemTag() {
            return $"nvidia_load_{_tag}";
        }
    }
}
