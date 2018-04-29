using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Settings
{
    public abstract class BaseModuleSettings : BaseSettings
    {
        private BaseModule _module;

        protected BaseModuleSettings() { }

        internal void ApplyModule(BaseModule module) {
            FileName = "module-" + SettingsManager.FixFileName(module.GetLogIdentifer()).ToLower();
            _module = module;
        }

        public abstract void GenerateDefault();
    }
}
