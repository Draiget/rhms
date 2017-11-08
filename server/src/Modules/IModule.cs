using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace server.Modules
{
    public interface IModule
    {
        bool OpenModule(ICollectingServer server);

        void CloseModule();

        HardwareType GetServicedHardwareType();

        List<HardwareIdentifer> GetApplyableHardware();

        string GetLogIdentifer();
    }
}
