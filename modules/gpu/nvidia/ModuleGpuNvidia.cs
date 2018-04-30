using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security;
using gpu_nvidia.Api;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using gpu_nvidia.Hardware;
using server.Modules;
using server.Modules.Base;
using server.Utils;
using server.Utils.Logging;

namespace gpu_nvidia
{
    public class ModuleGpuNvidia : BaseHardwareModule
    {
        public static ModuleGpuNvidia ModuleHandle;

        public static BaseModuleLoader Loader;
        public static BaseModuleLogger Logger;
        private static BaseCollectingServer _server;

        public ModuleGpuNvidia(BaseModuleLoader loader)
            : base(loader)
        {
            Loader = loader;
            Logger = GetLogger();
            _server = loader.GetServer();
            ModuleHandle = this;
        }

        public override string GetLogIdentifer(){
            return "NvidiaGPU";
        }

        public override string GetName(){
            return "NVIDIA GPU Module";
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public override bool InitializeHardware() {
            Logger.Debug("Initializing hardware ...");

            if (NvApi.NvApiEnumPhysicalGpUs == null || NvApi.NvApiGetPhysicalGpUsFromDisplay == null || NvApi.NvApiEnumNvidiaDisplayHandle == null) {
                Logger.Debug("Unable to init hardware, NvApiEnumPhysicalGpUs, NvApiEnumNvidiaDisplayHandle and NvApiGetPhysicalGpUsFromDisplay are null pointer");
                return false;
            }

            NvStatus status;
            var handles = new NvPhysicalGpuHandle[NvApi.MaxPhysicalGpus];
            if ((status = NvApi.NvApiEnumPhysicalGpUs(handles, out var gpuCount)) != NvStatus.Ok) {
                Logger.Error($"Unable to enumerate physical gpus, the API status is: {status}");
                return false;
            }

            for (var gpuIndex = 0; gpuIndex < gpuCount; gpuIndex++) {
                var ngpu = new NVidiaGpu(gpuIndex, handles[gpuIndex]);
                Logger.Debug($"Adding GPU {ngpu}");
                ngpu.InitializeSensors();
                Hardware.Add(ngpu);
            }

            return true;
        }

        public override bool CheckForSystemSupport() {
            return NvApi.CheckIsApiDllExists();
        }

        public override string GetUnsupportedReason() {
            return $"nVidia api dll '{NvApi.DllName}' is not found in your system";
        }

        public override bool Open(){
            if (!NvApi.Initialize()) {
                Logger.Error("NvAPI initialization failed");
                return false;
            }

            if (NvApi.ApiStatus == NvStatus.NvidiaDeviceNotFound) {
                Logger.Info("No nVidia devices present");
                return true;
            }

            if (NvApi.IsAvailable) {
                return InitializeHardware();
            }

            return false;
        }

        public override void Close(){
            return;
        }

        public override void ScheduledModuleUpdateTick() {
            ExportDataToGrafana(_server);
        }
    }
}
