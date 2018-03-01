using System.Runtime.InteropServices;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NvDynamicPerformanceStateUtilizationDomainInfo
    {
        internal readonly uint _IsPresent;
        internal readonly uint _Percentage;

        public bool IsPresent => _IsPresent.GetBit(0);

        public uint Percentage => _Percentage;

        public override string ToString() {
            return IsPresent ? $"{Percentage}%" : "Not Present";
        }
    }
}
