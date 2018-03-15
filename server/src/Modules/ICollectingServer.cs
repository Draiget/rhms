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
    public interface ICollectingServer
    {
        RhmsSettings GetSettings();

        bool HasActiveRemoteConnection();

        BaseModuleLoader GetModuleLoader();

        void Shutdown();

        void RegisterHardware(IHardware hardware);
    }
}
