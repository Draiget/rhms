using server.Utils.Logging;

namespace server.Modules.Base
{
    public abstract class BaseModule : IModule
    {
        private readonly BaseModuleLoader _moduleLoader;
        private readonly BaseModuleLogger _moduleLogger;

        protected BaseModule(BaseModuleLoader loader){
            _moduleLoader = loader;
            _moduleLogger = new BaseModuleLogger(this);
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

        public BaseModuleLogger GetLogger(){
            return _moduleLogger;
        }

        public override string ToString(){
            return $"BaseModule[logId='{GetLogIdentifer()}', name='{GetName()}']";
        }
    }
}
