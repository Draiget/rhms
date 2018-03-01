using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;
using server.Modules.Base;
using server.Utils.Logging;

namespace server.Modules
{
    public interface IModule
    {
        string GetLogIdentifer();

        string GetName();

        ModuleState GetState();

        BaseModuleLoader GetLoader();

        bool Open();

        void Close();

        BaseModuleLogger GetLogger();
    }
}
