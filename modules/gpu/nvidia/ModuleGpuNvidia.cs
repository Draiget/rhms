using gpu_nvidia.Api;
using gpu_nvidia.Api.Enums;
using server.Modules.Base;
using server.Utils.Logging;

namespace gpu_nvidia
{
    public class ModuleGpuNvidia : BaseModule
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
