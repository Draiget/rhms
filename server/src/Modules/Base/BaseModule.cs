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

        public ModuleState GetState(){
            return ModuleState.Unknown;
        }

        public BaseModuleLoader GetLoader(){
            return _moduleLoader;
        }

        public abstract bool Open();

        public abstract void Close();

        public abstract bool InitializeHardware();

        public virtual bool CheckForSystemSupport() {
            return true;
        }

        public virtual string GetUnsupportedReason() {
            return "unknown";
        }

        public List<IHardware> GetHardware() {
            return Hardware;
        }

        public BaseModuleLogger GetLogger(){
            return _moduleLogger;
        }

        public override string ToString(){
            return $"BaseModule[logId='{GetLogIdentifer()}', name='{GetName()}']";
        }

        public void AddHardware(IHardware hardware) {
            Hardware.Add(hardware);
        }
    }
}
