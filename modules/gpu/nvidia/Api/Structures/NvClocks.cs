using System.Runtime.InteropServices;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NvClocks
    {
        public uint Version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvApi.MaxClocksPerGpu)]
        public uint[] Clock;
    }
}
