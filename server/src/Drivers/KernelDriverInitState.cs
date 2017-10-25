using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers
{
    public enum KernelDriverInitState
    {
        RhmsDriverManagerUnknownErr = -2,

        RhmsDrvUnknownError = -1,
        RhmsDrvNoError = 0,
        RhmsDrvUnsupportedPlatform = 1,
        RhmsDrvDriverNotLoaded = 2,
        RhmsDrvDriverNotFound = 3,
        RhmsDrvDriverUnloaded = 4,
        RhmsDrvDriverNotLoadedOnNetwork = 5,

        RhmsDriverManagerOk = 10,
        RhmsDriverManagerIdOrPathEmpty = 11,
        RhmsDriverManagerOpenscmFailed = 12,
        RhmsDriverManagerIncorrectDrvSignature = 13
    }
}
