using System.Runtime.InteropServices;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvGpuThermalSettings
    {
        public uint Version;
        public uint Count;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NvApi.MaxThermalSensorsPerGpu)]
        public NvSensor[] Sensor;
    }
}
