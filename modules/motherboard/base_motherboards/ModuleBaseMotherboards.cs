using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;
using server.Modules.Base;
using server.Utils.Logging;

namespace base_motherboards
{
    public class ModuleBaseMotherboards : BaseHardwareModule
    {
        public static ModuleBaseMotherboards ModuleHandle;

        public static BaseModuleLoader Loader;
        public static BaseModuleLogger Logger;
        private static BaseCollectingServer _server;

        public ModuleBaseMotherboards(BaseModuleLoader loader)
            : base(loader) {
            Loader = loader;
            Logger = GetLogger();
            _server = loader.GetServer();
            ModuleHandle = this;
        }

        public override string GetLogIdentifer() {
            return "BaseMotherboards";
        }

        public override string GetName() {
            return "Base Motherboards";
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
                

                return true;
            } catch (Exception e) {
                Logger.Error("Unable to initialize chipset hardware", e);
                return false;
            }
        }
    }
}
