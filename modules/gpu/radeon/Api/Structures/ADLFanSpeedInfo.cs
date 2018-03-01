using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gpu_radeon.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AdlFanSpeedInfo
    {
        public int Size;
        public int Flags;
        public int MinPercent;
        public int MaxPercent;
        public int MinRPM;
        public int MaxRPM;
    }
}
