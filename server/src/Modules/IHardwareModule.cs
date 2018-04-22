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
    public interface IHardwareModule : IModule
    {
        bool InitializeHardware();

        void AddHardware(IHardware hardware);

        List<IHardware> GetHardware();
    }
}
