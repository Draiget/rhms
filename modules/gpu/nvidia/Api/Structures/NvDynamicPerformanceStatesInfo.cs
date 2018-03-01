using System.Runtime.InteropServices;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvDynamicPerformanceStatesInfo
    {
        internal const int MaxGpuUtilizations = 8;

        internal readonly uint _Flags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxGpuUtilizations)]
        internal NvDynamicPerformanceStateUtilizationDomainInfo[] UtilizationDomain;

        public bool IsDynamicPerformanceStateEnabled => _Flags.GetBit(0);

        /// Graphic engine (GPU) utilization
        public NvDynamicPerformanceStateUtilizationDomainInfo GpuDomain => UtilizationDomain[0];

        /// Frame buffer (FB) utilization
        public NvDynamicPerformanceStateUtilizationDomainInfo FrameBufferDomain => UtilizationDomain[1];

        /// Video engine (VID) utilization
        public NvDynamicPerformanceStateUtilizationDomainInfo VideoEngineDomain => UtilizationDomain[2];

        /// Bus interface (BUS) utilization
        public NvDynamicPerformanceStateUtilizationDomainInfo BusDomain => UtilizationDomain[3];

        public override string ToString() {
            return $"GPU = {GpuDomain} - FrameBuffer = {FrameBufferDomain} - VideoEngine = {VideoEngineDomain} - BusInterface = {BusDomain}";
        }
    }
}
