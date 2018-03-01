﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using gpu_nvidia.Api.Delegates;
using gpu_nvidia.Api.Enums;
using gpu_nvidia.Api.Structures;
using gpu_nvidia.Helpers;
using server.Modules.Helpers;
using server.Utils;
using FunctionId = gpu_nvidia.Helpers.FunctionId;

namespace gpu_nvidia.Api
{
    public static class NvApi
    {
        public static readonly uint GpuThermalSettingsVer = (uint)Marshal.SizeOf(typeof(NvGpuThermalSettings)) | 0x10000;
        public static readonly uint GpuClocksVer = (uint)Marshal.SizeOf(typeof(NvClocks)) | 0x20000;
        /*public static readonly uint GpuPstatesVer = (uint)Marshal.SizeOf(typeof(NvPStates)) | 0x10000;
        public static readonly uint GpuUsagesVer = (uint)Marshal.SizeOf(typeof(NvUsages)) | 0x10000;
        public static readonly uint GpuCoolerSettingsVer = (uint) Marshal.SizeOf(typeof(NvGpuCoolerSettings)) | 0x20000;
        public static readonly uint GpuMemoryInfoVer = (uint)Marshal.SizeOf(typeof(NvMemoryInfo)) | 0x20000;
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
        private static readonly NvApiQueryInterfaceDelegate NvApiQueryInterface;
        private static readonly NvApiInitializeDelegate NvApiInitialize;

        [LinkedFunctionId(FunctionId.NvAPI_EnumPhysicalGPUs)]
        public static NvApiGpu.NvAPI_EnumPhysicalGPUs NvApiEnumPhysicalGpUs;

        private delegate IntPtr NvApiQueryInterfaceDelegate(uint id);
        private delegate NvStatus NvApiInitializeDelegate();

        public static bool Initialize() {
            if (NvApiInitialize == null) {
                return false;
            }

            _initStatus = NvApiInitialize();
            //if (_initStatus == NvStatus.Ok) {
                SetupDelegates();
            //}

            return true;
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

        static NvApi(){
            var attribute = new DllImportAttribute(DllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = "nvapi_QueryInterface"
            };

            Native.CreatePInvokeDelegate(attribute, out NvApiQueryInterface);

            try {
                GetDelegate((uint) FunctionId.NvAPI_Initialize, out NvApiInitialize);
            }
            catch (DllNotFoundException e) {
                ModuleGpuNvidia.Logger.Error($"Could not found nvapi dll '{DllName}'", e);
            }
            catch (EntryPointNotFoundException e) {
                ModuleGpuNvidia.Logger.Error($"Could not found entry point in '{DllName}'", e);
            }
            catch (ArgumentNullException e) {
                ModuleGpuNvidia.Logger.Error($"Could not query pointer to NvAPI_Initialize in '{DllName}'", e);
            }
        }

        private static string DllName = IntPtr.Size == 4 ? "nvapi.dll" : "nvapi64.dll";

        public static bool IsAvailable => _initStatus == NvStatus.Ok;

        public static NvStatus ApiStatus => _initStatus;

        private static void GetDelegate<T>(uint id, out T newDelegate) where T : class {
            var ptr = NvApiQueryInterface(id);
            if (ptr != IntPtr.Zero) {
                newDelegate = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            } else {
                newDelegate = null;
            }
        }

        private static object ApplyDelegate(uint id, FieldInfo field) {
            var ptr = NvApiQueryInterface(id);
            if (ptr != IntPtr.Zero) {
                return Marshal.GetDelegateForFunctionPointer(ptr, field.FieldType);
            }

            return null;
        }
    }
}