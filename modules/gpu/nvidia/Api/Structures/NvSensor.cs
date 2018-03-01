using gpu_nvidia.Api.Enums;

namespace gpu_nvidia.Api.Structures
{
    public struct NvSensor
    {
        public NvThermalController Controller;
        public uint DefaultMinTemp;
        public uint DefaultMaxTemp;
        public uint CurrentTemp;
        public NvThermalTarget Target;
    }
}
