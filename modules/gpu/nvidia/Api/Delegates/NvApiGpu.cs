using System.Runtime.InteropServices;
using System.Text;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using gpu_nvidia.Helpers;

// ReSharper disable InconsistentNaming
namespace gpu_nvidia.Api.Delegates
{
    public static class NvApiGpu
    {
        [FunctionId(FunctionId.NvAPI_EnumPhysicalGPUs)]
        public delegate NvStatus NvAPI_EnumPhysicalGPUs(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = NvPhysicalGpuHandle.MaxPhysicalGPUs)] NvPhysicalGpuHandle[]
                gpuHandles, [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_GetPhysicalGPUsFromDisplay)]
        public delegate NvStatus NvAPI_GetPhysicalGPUsFromDisplay(
            NvDisplayHandle displayHandle, [Out] NvPhysicalGpuHandle[] gpuHandles,
            out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_EnumNvidiaDisplayHandle)]
        public delegate NvStatus NvAPI_EnumNvidiaDisplayHandle(
            int thisEnum,
            ref NvDisplayHandle displayHandle);

        [FunctionId(FunctionId.NvAPI_EnumTCCPhysicalGPUs)]
        public delegate NvStatus NvAPI_EnumTCCPhysicalGPUs(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = NvPhysicalGpuHandle.MaxPhysicalGPUs)] NvPhysicalGpuHandle[]
                gpuHandles, [Out] out uint gpuCount);

        [FunctionId(FunctionId.NvAPI_GPU_GetAllClocks)]
        public delegate NvStatus NvAPI_GPU_GetAllClocks(
            NvPhysicalGpuHandle gpuHandle, ref NvClocks nvClocks);

        [FunctionId(FunctionId.NvAPI_GPU_GetMemoryInfo)]
        public delegate NvStatus NvAPI_GPU_GetMemoryInfo(
            NvDisplayHandle displayHandle, ref NvMemoryInfo nvMemoryInfo);

        [FunctionId(FunctionId.NvAPI_GPU_GetAGPAperture)]
        public delegate NvStatus NvAPI_GPU_GetAGPAperture(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint size);

        [FunctionId(FunctionId.NvAPI_GPU_GetBoardInfo)]
        public delegate NvStatus NvAPI_GPU_GetBoardInfo(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] [In] ref NvBoardInfo info);

        [FunctionId(FunctionId.NvAPI_GPU_GetBusId)]
        public delegate NvStatus NvAPI_GPU_GetBusId(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint gpuBusId);

        [FunctionId(FunctionId.NvAPI_GPU_GetBusSlotId)]
        public delegate NvStatus NvAPI_GPU_GetBusSlotId(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint gpuBusSlotId);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentAGPRate)]
        public delegate NvStatus NvAPI_GPU_GetCurrentAGPRate(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint rate);

        [FunctionId(FunctionId.NvAPI_GPU_GetCurrentPCIEDownstreamWidth)]
        public delegate NvStatus NvAPI_GPU_GetCurrentPCIEDownstreamWidth(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint width);

        [FunctionId(FunctionId.NvAPI_GPU_GetDynamicPStatesInfoEx)]
        public delegate NvStatus NvAPI_GPU_GetDynamicPStatesInfoEx(
            [In] NvPhysicalGpuHandle physicalGpu,
            [In] NvDynamicPerformanceStatesInfo performanceStatesInfoEx);

        [FunctionId(FunctionId.NvAPI_GPU_GetFullName)]
        public delegate NvStatus NvAPI_GPU_GetFullName(
            NvPhysicalGpuHandle gpuHandle, StringBuilder name);

        [FunctionId(FunctionId.NvAPI_GPU_GetGpuCoreCount)]
        public delegate NvStatus NvAPI_GPU_GetGpuCoreCount(
            [In] NvPhysicalGpuHandle gpuHandle,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetGPUType)]
        public delegate NvStatus NvAPI_GPU_GetGPUType(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out NvGPUType gpuType);

        [FunctionId(FunctionId.NvAPI_GPU_GetIRQ)]
        public delegate NvStatus NvAPI_GPU_GetIRQ(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint gpuIRQ);

        [FunctionId(FunctionId.NvAPI_GPU_GetPCIIdentifiers)]
        public delegate NvStatus NvAPI_GPU_GetPCIIdentifiers(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint deviceId, [Out] out uint subSystemId,
            [Out] out uint revisionId, [Out] out uint extDeviceId);

        [FunctionId(FunctionId.NvAPI_GPU_GetPhysicalFrameBufferSize)]
        public delegate NvStatus NvAPI_GPU_GetPhysicalFrameBufferSize(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint size);

        [FunctionId(FunctionId.NvAPI_GPU_GetQuadroStatus)]
        public delegate NvStatus NvAPI_GPU_GetQuadroStatus(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint isQuadro);

        [FunctionId(FunctionId.NvAPI_GPU_GetShaderSubPipeCount)]
        public delegate NvStatus NvAPI_GPU_GetShaderSubPipeCount(
            [In] NvPhysicalGpuHandle gpuHandle,
            [Out] out uint count);

        [FunctionId(FunctionId.NvAPI_GPU_GetThermalSettings)]
        public delegate NvStatus NvAPI_GPU_GetThermalSettings(
            NvPhysicalGpuHandle gpuHandle, int sensorIndex,
            ref NvGpuThermalSettings nvGPUThermalSettings);

        [FunctionId(FunctionId.NvAPI_GPU_GetVbiosOEMRevision)]
        public delegate NvStatus NvAPI_GPU_GetVbiosOEMRevision(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint biosOEMRevision);

        [FunctionId(FunctionId.NvAPI_GPU_GetVbiosRevision)]
        public delegate NvStatus NvAPI_GPU_GetVbiosRevision(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint biosRevision);

        [FunctionId(FunctionId.NvAPI_GPU_GetVbiosVersionString)]
        public delegate NvStatus NvAPI_GPU_GetVbiosVersionString(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out StringBuilder biosVersion);

        [FunctionId(FunctionId.NvAPI_GPU_GetVirtualFrameBufferSize)]
        public delegate NvStatus NvAPI_GPU_GetVirtualFrameBufferSize(
            [In] NvPhysicalGpuHandle physicalGpu, [Out] out uint size);

        [FunctionId(FunctionId.NvAPI_SYS_GetPhysicalGpuFromDisplayId)]
        public delegate NvStatus NvAPI_SYS_GetPhysicalGpuFromDisplayId(
            [In] uint displayId, [Out] out NvPhysicalGpuHandle gpu);
    }
}
