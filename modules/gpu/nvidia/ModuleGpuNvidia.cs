﻿using gpu_nvidia.Api;
using gpu_nvidia.Api.Enums;
using server.Modules;
using server.Modules.Base;
using server.Utils;
using server.Utils.Logging;

namespace gpu_nvidia
{
    public class ModuleGpuNvidia : BaseHardwareModule
    {
        public static BaseModuleLoader Loader;
        public static BaseModuleLogger Logger;

        public ModuleGpuNvidia(BaseModuleLoader loader)
            : base(loader)
        {
            Loader = loader;
            Logger = GetLogger();
        }

        public override string GetLogIdentifer(){
            return "NvidiaGPU";
        }

        public override string GetName(){
            return "NVIDIA GPU Module";
        }

        public override bool InitializeHardware() {
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

            return NvApi.IsAvailable;
        }

        public override void Close(){
            return;
        }
    }
}
