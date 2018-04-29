using System.Collections;
using System.Collections.Generic;
using server.Hardware;
using server.Settings;
using server.Utils.Logging;

namespace server.Modules.Base
{
    public abstract class BaseModule : IModule
    {
        private readonly BaseModuleLoader _moduleLoader;
        private readonly BaseModuleLogger _moduleLogger;

        protected List<IHardware> Hardware;

        protected BaseModule(BaseModuleLoader loader){
            _moduleLoader = loader;
            _moduleLogger = new BaseModuleLogger(this);
            Hardware = new List<IHardware>();
        }

        public virtual string GetLogIdentifer(){
            return "<Unknown>";
        }

        public virtual string GetName(){
            return GetLogIdentifer();
        }

        /// <summary>
        /// Retreive state of the current module (Loaded, Loading, LoadingError, Unloaded, etc.)
        /// </summary>
        /// <returns>Current module state</returns>
        public ModuleState GetState(){
            return ModuleState.Unknown;
        }

        /// <summary>
        /// Return main rhms collecting server module loader
        /// </summary>
        /// <returns>Module loader instance</returns>
        public BaseModuleLoader GetLoader(){
            return _moduleLoader;
        }

        /// <summary>
        /// Custom logic when module is loading up by rhms server module loader
        /// </summary>
        /// <returns>State if module is open successfully or failed</returns>
        public abstract bool Open();

        /// <summary>
        /// Custom logic when module is unloading by rhms server module loader
        /// </summary>
        /// <returns>State if module is unloaded successfully or failed (module will be unloaded anyway, return status tells only that module unloading logic are failed with some error)</returns>
        public abstract void Close();

        /// <summary>
        /// Check if remote system can support this module, or hardware that this module exposes and presents can be using in this particular system
        /// </summary>
        /// <returns>System support module status</returns>
        public virtual bool CheckForSystemSupport() {
            return true;
        }

        internal BaseModuleSettings SettingsInstance;

        public virtual dynamic GetSettingsType() {
            return null;
        }

        public BaseModuleSettings Settings {
            get => _moduleLoader.GetSettingsFor(this);
            set => _moduleLoader.SaveSettingsFor(this, value);
        }

        public SettingsIoResult? SaveSettings() {
            return _moduleLoader.SaveSettingsFor(this);
        }

        /// <summary>
        /// Returns reason why this module isn't supported in this specific system
        /// </summary>
        /// <returns>Reson string</returns>
        public virtual string GetUnsupportedReason() {
            return "unknown";
        }
        
        /// <summary>
        /// Getting RHMS Collecting server logger instance (for specific module, instance will use module configuration, like module name and logger identifier)
        /// </summary>
        /// <returns>Logger instance</returns>
        public BaseModuleLogger GetLogger(){
            return _moduleLogger;
        }

        public override string ToString(){
            return $"BaseModule[logId='{GetLogIdentifer()}', name='{GetName()}']";
        }
    }
}
