﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Modules
{
    public enum ModuleState
    {
        Unknown = -1,

        Unloaded,
        Loading,
        Loaded,
        Unloading,

        LoadingError,
        UnloadingError
    }
}
