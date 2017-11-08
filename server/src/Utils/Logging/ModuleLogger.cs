using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules;

namespace server.Utils.Logging
{
    public class ModuleLogger : ICollectingServerLogger
    {
        private readonly IModule _currentModule;

        public ModuleLogger(IModule module){
            _currentModule = module;
        }

        public void Debug(string msg){
            Logger.Debug($"[{_currentModule.GetLogIdentifer()}] {msg}");
        }

        public void Info(string msg){
            Logger.Info($"[{_currentModule.GetLogIdentifer()}] {msg}");
        }

        public void Warn(string msg){
            Logger.Warn($"[{_currentModule.GetLogIdentifer()}] {msg}");
        }

        public void Error(string msg, Exception err = null) {
            Logger.Error($"[{_currentModule.GetLogIdentifer()}] {msg}", err);
        }
    }
}
