using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gpu_radeon.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AdlTemperature
    {
        public int Size;
        public int Temperature;
    }
}
