using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NvDisplayHandle
    {
        private readonly IntPtr ptr;

        public override string ToString() {
            return $"NvDisplayHandle [ptr={ptr}]";
        }
    }
}
