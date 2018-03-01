namespace gpu_nvidia.Api.Enums
{
    public enum NvThermalTarget
    {
        None = 0,
        Gpu = 1,
        Memory = 2,
        PowerSupply = 4,
        Board = 8,
        All = 15,
        Unknown = -1
    }
}
