﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gpu_radeon.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AdlFanSpeedValue
    {
        public int Size;
        public int SpeedType;
        public int FanSpeed;
        public int Flags;
    }
}
