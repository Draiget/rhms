using System.Collections;
using System.Collections.Generic;
using server.Hardware;
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
        /// Method is calling after all hardware are updated and information collected.
        /// May be used to store sensors and hardware data to database, or use it with custom influx API handlers
        /// </summary>
        public virtual void AfterHardwareTick() { }

        /// <summary>
        /// Specific hardware initialization stage, can be overriden and using for loading external API or calling exporing functions
        /// </summary>
        /// <returns>State if all hardware that supports this module are loaded well</returns>
        public abstract bool InitializeHardware();

        /// <summary>
        /// Check if remote system can support this module, or hardware that this module exposes and presents can be using in this particular system
        /// </summary>
        /// <returns>System support module status</returns>
        public virtual bool CheckForSystemSupport() {
            return true;
        }

        /// <summary>
        /// Returns reason why this module isn't supported in this specific system
        /// </summary>
        /// <returns>Reson string</returns>
        public virtual string GetUnsupportedReason() {
            return "unknown";
        }
        
        /// <summary>
        /// Retreive all hardware that this module can support and operate with
        /// </summary>
        /// <returns>List of hardware interfaces</returns>
        public List<IHardware> GetHardware() {
            return Hardware;
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

        /// <summary>
        /// <p>Adds hardware that can be supported with this specific module.</p>
        /// <p>Hardware can't be removed from this list, use <see cref="InitializeHardware"/> to prevent module loading</p>
        /// </summary>
        /// <param name="hardware"></param>
        public void AddHardware(IHardware hardware) {
            Hardware.Add(hardware);
        }
    }
}
