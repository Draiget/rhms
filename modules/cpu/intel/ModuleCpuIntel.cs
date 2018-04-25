using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cpu_intel.Api.Hardware;
using server.Drivers.Presentation;
using server.Modules.Base;
using server.Utils.Logging;

namespace cpu_intel
{
    public class ModuleCpuIntel : BaseHardwareModule
    {
        public static ModuleCpuIntel ModuleHandle;

        public static BaseModuleLoader Loader;
        public static BaseModuleLogger Logger;
        private static BaseCollectingServer _server;

        public ModuleCpuIntel(BaseModuleLoader loader)
            : base(loader) 
        {
            Loader = loader;
            Logger = GetLogger();
            _server = loader.GetServer();
            ModuleHandle = this;
        }

        public override string GetLogIdentifer() {
            return "IntelCPU";
        }

        public override string GetName() {
            return "Intel CPU's Module";
        }

        public override bool Open() {
            try {
                InitializeHardware();
            } catch (Exception e) {
                Logger.Error("Unable to initialize module hardware", e);
                return false;
            }

            return true;
        }

        public override void Close() {
            // No actions need
        }

        public override bool InitializeHardware() {
            try {
                var cpuidsByProcessor = Cpuid.Get();
                foreach (var cpuId in cpuidsByProcessor.Keys) {
                    var cpu = new IntelCpu(cpuidsByProcessor[cpuId]);
                    cpu.InitializeSensors();
                    AddHardware(cpu);
                }
                
                return true;
            } catch (Exception e) {
                Logger.Error("Unable to initialize CPUID hardware", e);
                return false;
            }
        }
    }
}
