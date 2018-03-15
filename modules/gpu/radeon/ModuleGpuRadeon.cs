using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api;
using gpu_radeon.Api.Hardware;
using gpu_radeon.Api.Structures;
using server.Modules.Base;
using server.Utils.Logging;

namespace gpu_radeon
{
    public class ModuleGpuRadeon : BaseModule
    {
        public static BaseModuleLoader Loader;
        public static BaseModuleLogger Logger;

        public ModuleGpuRadeon(BaseModuleLoader loader)
            : base(loader)
        {
            Loader = loader;
            Logger = GetLogger();
        }

        public override string GetLogIdentifer() {
            return "RadeonGPU";
        }

        public override string GetName() {
            return "ATI/Radeon GPU Module";
        }

        public override bool Open(){
            if (!AdlApi.Initialize()) {
                return false;
            }

            try {
                InitializeHardware();
            } catch (Exception e) {
                Logger.Error("Unable to initialize module hardware", e);
                return false;
            }

            return true;
        }

        public override void Close(){
            AdlApi.Shutdown();
        }

        public override bool InitializeHardware() {
            var adaptersCount = 0;
            AdlApi.AdlGetNumberOfAdapters(ref adaptersCount);

            if (adaptersCount <= 0) {
                return false;
            }

            if (AdlApi.AldAdapterGetInfo(adaptersCount, out var adaptersInfo) != AdlApi.AdlOk) {
                return false;
            }

            for (var i=0; i < adaptersCount; i++) {
                if (string.IsNullOrEmpty(adaptersInfo[i].UDID) || adaptersInfo[i].VendorID != AdlApi.AtiVendorId) {
                    continue;
                }

                if (Hardware.Any(hwd => hwd is RadeonGpu && ((RadeonGpu)hwd).IsSameDeviceAs(adaptersInfo[i]) )) {
                    continue;
                }

                var radeonAdapter = new RadeonGpu(adaptersInfo[i]);
                ProcessAdlAdapter(radeonAdapter);
                AddHardware(radeonAdapter);
            }

            return true;
        }

        private void ProcessAdlAdapter(RadeonGpu gpu) {
            AdlApi.AdlGetAdapterIsActive(gpu.AdapterIndex, out var isActive);
            AdlApi.AdlAdapterGetId(gpu.AdapterIndex, out var adapterId);

            gpu.SetActive(isActive == 1);
            gpu.SetAdapterId(adapterId);
        }
    }
}
