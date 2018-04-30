using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvPState
    {
        public bool Present;
        public int Percentage;
    }
}
