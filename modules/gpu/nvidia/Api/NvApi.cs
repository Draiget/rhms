using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using gpu_nvidia.Api.Delegates;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using gpu_nvidia.Helpers;
using server.Modules.Helpers;
using server.Utils;
using server.Utils.Logging;
using FunctionId = gpu_nvidia.Helpers.FunctionId;

namespace gpu_nvidia.Api
{
    public static class NvApi
    {
        public static readonly uint GpuThermalSettingsVer = (uint)Marshal.SizeOf(typeof(NvGpuThermalSettings)) | 0x10000;
        public static readonly uint GpuClocksVer = (uint)Marshal.SizeOf(typeof(NvClocks)) | 0x20000;
        public static readonly uint GpuMemoryInfoVer = (uint)Marshal.SizeOf(typeof(NvMemoryInfo)) | 0x20000;
        /*public static readonly uint GpuPstatesVer = (uint)Marshal.SizeOf(typeof(NvPStates)) | 0x10000;
        public static readonly uint GpuUsagesVer = (uint)Marshal.SizeOf(typeof(NvUsages)) | 0x10000;
        public static readonly uint GpuCoolerSettingsVer = (uint) Marshal.SizeOf(typeof(NvGpuCoolerSettings)) | 0x20000;
        public static readonly uint DisplayDriverVersionVer = (uint) Marshal.SizeOf(typeof(NvDisplayDriverVersion)) | 0x10000;
        public static readonly uint GpuCoolerLevelsVer = (uint)Marshal.SizeOf(typeof(NvGpuCoolerLevels)) | 0x10000;*/

        public const int MaxPhysicalGpus = 64;
        public const int ShortStringMax = 64;

        public const int MaxThermalSensorsPerGpu = 3;
        public const int MaxClocksPerGpu = 0x120;
        public const int MaxPstatesPerGpu = 8;
        public const int MaxUsagesPerGpu = 33;
        public const int MaxCoolerPerGpu = 20;
        public const int MaxMemoryValuesPerGpu = 5;

        private static NvStatus _initStatus;
        private static NvApiQueryInterfaceDelegate _nvApiQueryInterface;
        private static NvApiInitializeDelegate _nvApiInitialize;

        [LinkedFunctionId(FunctionId.NvAPI_EnumPhysicalGPUs)]
        public static NvApiGpu.NvAPI_EnumPhysicalGPUs NvApiEnumPhysicalGpUs;

        [LinkedFunctionId(FunctionId.NvAPI_GetPhysicalGPUsFromDisplay)]
        public static NvApiGpu.NvAPI_GetPhysicalGPUsFromDisplay NvApiGetPhysicalGpUsFromDisplay;

        [LinkedFunctionId(FunctionId.NvAPI_EnumNvidiaDisplayHandle)]
        public static NvApiGpu.NvAPI_EnumNvidiaDisplayHandle NvApiEnumNvidiaDisplayHandle;

        [LinkedFunctionId(FunctionId.NvAPI_GPU_GetThermalSettings)]
        public static NvApiGpu.NvAPI_GPU_GetThermalSettings GetThermalSettings;

        [LinkedFunctionId(FunctionId.NvAPI_GPU_GetAllClocks)]
        public static NvApiGpu.NvAPI_GPU_GetAllClocks GetAllClocks;

        [LinkedFunctionId(FunctionId.NvAPI_GPU_GetMemoryInfo)]
        public static NvApiGpu.NvAPI_GPU_GetMemoryInfo GetMemoryInfo;

        [LinkedFunctionId(FunctionId.NvAPI_GPU_GetFullName)]
        public static NvApiGpu.NvAPI_GPU_GetFullName GpuGetFullName;

        private delegate IntPtr NvApiQueryInterfaceDelegate(uint id);
        private delegate NvStatus NvApiInitializeDelegate();

        public static bool Initialize() {
            if (_nvApiInitialize != null) {
                return true;
            }

            var attribute = new DllImportAttribute(DllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = "nvapi_QueryInterface"
            };

            Native.CreatePInvokeDelegate(attribute, out _nvApiQueryInterface);

            try {
                GetDelegate((uint)FunctionId.NvAPI_Initialize, out _nvApiInitialize);

                _initStatus = _nvApiInitialize();
                if (_initStatus == NvStatus.Ok) {
                    SetupDelegates();
                }
                return true;
            } catch (DllNotFoundException e) {
                ModuleGpuNvidia.Logger.Error($"Could not found nvapi dll '{DllName}'", e);
            } catch (EntryPointNotFoundException e) {
                ModuleGpuNvidia.Logger.Error($"Could not found entry point in '{DllName}'", e);
            } catch (ArgumentNullException e) {
                ModuleGpuNvidia.Logger.Error($"Could not query pointer to NvAPI_Initialize in '{DllName}'", e);
            }

            return false;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static NvStatus NvApiGpuGetFullName(NvPhysicalGpuHandle handle, out string name) {
            if (GpuGetFullName == null) {
                name = "Unknown";
                return NvStatus.FunctionNotFound;
            }

            try {
                var builder = new StringBuilder(short.MaxValue);
                var status = GpuGetFullName(handle, builder);
                name = builder.ToString();
                return status;
            } catch (AccessViolationException ave) {
                Logger.Error($"Unable to get GPU name from handle {handle}", ave);
                name = "Unknown";
                return NvStatus.FunctionNotFound;
            }
        }

        private static void SetupDelegates(){
            // Get all delegates with function id attribute
            var delegatesList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(FunctionIdAttribute)).Any());

            var fieldsList = new List<FieldInfo>();

            // Add all fields with delegate function id linking attribute to list for futhure iteration
            foreach (var assemblyType in Assembly.GetExecutingAssembly().GetTypes()) {
                fieldsList.AddRange(
                    from typeField in assemblyType.GetFields()
                    let fieldAttribute = typeField.GetCustomAttribute<LinkedFunctionIdBaseAttribute>()
                    where fieldAttribute != null
                    select typeField);
            }

            // Search target field for each delegate attributes
            foreach (var delegateContainerClass in delegatesList) {
                var attr = delegateContainerClass.GetCustomAttribute<FunctionIdAttribute>();
                if (attr == null) {
                    continue;
                }

                var hasLinkedField = false;

                foreach (var field in fieldsList) {
                    var linkedAttr = field.GetCustomAttribute<LinkedFunctionIdBaseAttribute>();
                    if (linkedAttr?.FunctionId == attr.FunctionId) {
                        hasLinkedField = true;

                        try {
                            field.SetValue(null, ApplyDelegate((uint) attr.FunctionId, field));
                            ModuleGpuNvidia.Logger.Debug($"Linked delegate field '{field.Name}' with function id '{attr.FunctionId}'");
                        }
                        catch (FieldAccessException e) {
                            ModuleGpuNvidia.Logger.Error($"Failed to linke delegate field '{field.Name}' with function id '{attr.FunctionId}'", e);
                        }
                    }
                }

                if (!hasLinkedField) {
                    ModuleGpuNvidia.Logger.Warn($"Delegate with function id '{attr.FunctionId}' is not linked (not using)");
                }
            }

            // Cleanup
            fieldsList.Clear();
        }

        public static bool CheckIsApiDllExists() {
            return Native.IfLibraryIsExists(DllName);
        }

        public static readonly string DllName = IntPtr.Size == 4 ? "nvapi.dll" : "nvapi64.dll";

        public static bool IsAvailable => _initStatus == NvStatus.Ok;

        public static NvStatus ApiStatus => _initStatus;

        private static void GetDelegate<T>(uint id, out T newDelegate) where T : class {
            var ptr = _nvApiQueryInterface(id);
            if (ptr != IntPtr.Zero) {
                newDelegate = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            } else {
                newDelegate = null;
            }
        }

        private static object ApplyDelegate(uint id, FieldInfo field) {
            var ptr = _nvApiQueryInterface(id);
            if (ptr != IntPtr.Zero) {
                return Marshal.GetDelegateForFunctionPointer(ptr, field.FieldType);
            }

            return null;
        }
    }
}
