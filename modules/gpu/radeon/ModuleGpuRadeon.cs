using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api;
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
            return AdlApi.Initialize();
        }

        public override void Close(){
            AdlApi.Shutdown();
        }
    }
}
