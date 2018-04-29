using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;
using server.Settings;

namespace gpu_radeon
{
    public class ModuleSettings : BaseModuleSettings
    {
        private readonly ModuleGpuRadeon _module;

        public ModuleSettings(ModuleGpuRadeon module) {
            _module = module;
        }

        public SectionFanInfo FanInfo;

        public override void GenerateDefault() {
            Default = new ModuleSettings(_module) {
                FanInfo = new SectionFanInfo()
            };
        }
    }

    public class SectionFanInfo : DynamicSettingsCategory
    {
        public override string GetDescription() {
            return "If section is enabled, fan sensors will be enabled too";
        }
    }
}
