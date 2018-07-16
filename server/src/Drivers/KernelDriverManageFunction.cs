using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers
{
    public enum KernelDriverManageFunction
    {
        RhmsDriverInstall = 1,
        RhmsDriverRemove = 2,
        RhmsDriverSystemInstall = 3,
        RhmsDriverSystemUninstall = 4,
        RhmsDisplayDriverStop = 5,
        RhmsDisplayDriverStart = 6
    }
}