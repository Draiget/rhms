using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using gpu_radeon.Api.Delegates;
using gpu_radeon.Api.Enums;
using gpu_radeon.Api.Helpers;
using gpu_radeon.Api.Structures;
using server.Utils;

namespace gpu_radeon.Api
{
    public static class AdlApi
    {
        private static bool _isInitialized;

        public const int AtiVendorId = 0x1002;

        public static bool Initialize(){
            AdlMainMemoryAlloc = Marshal.AllocHGlobal;

            LinkDelegates();

            var status = AdlMainControlCreate();
            if (status != AdlStatus.Ok) {
                ModuleGpuRadeon.Logger.Error($"Unable to initialize 'ADL_Main_Control_Create', return status: {status}");
                _isInitialized = false;
                return false;
            }

            _isInitialized = true;
            return true;
        }

        public static AdlApiGpu.ADL_Main_Memory_AllocDelegate AdlMainMemoryAlloc;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Main_Control_CreateDelegate AldMainControlCreate;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Main_Control_DestroyDelegate AdlMainControlDestroy;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Overdrive5_Temperature_GetDelegate AdlGetAdapterTemperature;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Adapter_Active_GetDelegate AdlGetAdapterIsActive;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Overdrive5_CurrentActivity_GetDelegate AdlGetCurrentActivity;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Adapter_NumberOfAdapters_GetDelegate AdlGetNumberOfAdapters;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Overdrive5_FanSpeed_GetDelegate AdlGetFanSpeed;

        [LinkedField(typeof(AdlApiGpu))]
        public static AdlApiGpu.ADL_Overdrive5_FanSpeedInfo_GetDelegate AdlGetFanSpeedInfo;

        // Warning disabled because this field are accessed dynamically thought LinkedField attribute
        #pragma warning disable 0649
        // ReSharper disable InconsistentNaming
        [LinkedField(typeof(AdlApiGpu))]
        internal static AdlApiGpu.ADL_Adapter_ID_GetDelegate _adlAdapterGetId;

        [LinkedField(typeof(AdlApiGpu))]
        internal static AdlApiGpu.ADL_Display_AdapterID_GetDelegate _adlDisplayAdapterGetId;

        [LinkedField(typeof(AdlApiGpu))]
        internal static AdlApiGpu.ADL_Adapter_AdapterInfo_GetDelegate _aldAdapterGetInfo;
        // ReSharper restore InconsistentNaming
        #pragma warning restore 0649

        public const int AdlMaxPath = 256;
        public const int AdlOk = 0;

        public static int AldAdapterGetInfo(int numberOfAdapters, out AdlAdapterInfo[] info) {
            var typeSize = Marshal.SizeOf(typeof(AdlAdapterInfo));
            var ptr = Marshal.AllocHGlobal(typeSize * numberOfAdapters);
            var result = _aldAdapterGetInfo(ptr, typeSize * numberOfAdapters);

            info = new AdlAdapterInfo[numberOfAdapters];
            for (var i=0; i < info.Length; i++) {
                info[i] = (AdlAdapterInfo) Marshal.PtrToStructure((IntPtr)((long)ptr + i * typeSize),typeof(AdlAdapterInfo));
            }

            Marshal.FreeHGlobal(ptr);

            // Maybe wrong on Windows subsystem (parsing error)
            FixAdvAdapterVendorInfo(ref info);

            return result;
        }

        public static int AdlAdapterGetId(int adapterIndex, out int adapterId) {
            try {
                return _adlAdapterGetId(adapterIndex, out adapterId);
            } catch (EntryPointNotFoundException) {
                try {
                    return _adlDisplayAdapterGetId(adapterIndex, out adapterId);
                } catch (EntryPointNotFoundException) {
                    adapterId = 1;
                    return AdlOk;
                }
            }
        }

        public static void FixAdvAdapterVendorInfo(ref AdlAdapterInfo[] info){
            for (var i=0; i < info.Length; i++) {
                // Try Windows UUID format first, it can fail
                var m = Regex.Match(info[i].UDID, "PCI_VEN_([A-Fa-f0-9]{1,4})&.*");;
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 16);
                    continue;
                }

                // Try Unix UUID format
                m = Regex.Match(info[i].UDID, "[0-9]+:[0-9]+:([0-9]+):[0-9]+:[0-9]+");
                if (m.Success && m.Groups.Count == 2) {
                    info[i].VendorID = Convert.ToInt32(m.Groups[1].Value, 10);
                }
            }
        }

        private static AdlStatus AdlMainControlCreate(int connectedAdapters = 1){
            try {
                return (AdlStatus)AldMainControlCreate(AdlMainMemoryAlloc, connectedAdapters);
            } catch (Exception err) {
                ModuleGpuRadeon.Logger.Error($"Unable ti create main ADL control: {err}");
                return AdlStatus.Err;
            }
        }

        private static void LinkDelegates(){
            var delegatesList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(LinkedDelegateAttribute)).Any());
            var fieldsList = new List<FieldInfo>();

            // Add all fields with delegate function id linking attribute to list for futhure iteration
            foreach (var assemblyType in Assembly.GetExecutingAssembly().GetTypes()) {
                fieldsList.AddRange(
                    from typeField in assemblyType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | 
                                                             BindingFlags.Instance | BindingFlags.Static)
                    let fieldAttribute = typeField.GetCustomAttribute<LinkedFieldAttribute>()
                    where fieldAttribute != null
                    select typeField);
            }

            var linkedFields = new Dictionary<FieldInfo,bool>();

            // Search target field for each delegate attributes
            foreach (var delegateFieldType in delegatesList) {
                var attr = delegateFieldType.GetCustomAttribute<LinkedDelegateAttribute>();
                if (attr == null) {
                    continue;
                }

                var hasLinkedField = false;

                foreach (var field in fieldsList) {
                    if (linkedFields.ContainsKey(field)) {
                        continue;
                    }

                    var linkedAttr = field.GetCustomAttribute<LinkedFieldAttribute>();
                    if (linkedAttr?.TargetLookType == delegateFieldType.ReflectedType && field.FieldType == delegateFieldType) {
                        hasLinkedField = true;

                        try {
                            field.SetValue(null, ApplyDelegate(attr.FunctionName, field));
                            linkedFields[field] = true;
                            ModuleGpuRadeon.Logger.Debug($"Linked delegate field '{field.Name}' with function '{delegateFieldType}' ({attr.FunctionName})");
                        } catch (FieldAccessException e) {
                            ModuleGpuRadeon.Logger.Error($"Failed to linke delegate field '{field.Name}' with function id '{delegateFieldType}' ({attr.FunctionName})", e);
                        }
                    }
                }

                if (!hasLinkedField) {
                    ModuleGpuRadeon.Logger.Warn($"Delegate with function '{delegateFieldType}' is not linked (not using)");
                }
            }

            // Cleanup
            fieldsList.Clear();
        }

        private static string DllName = "atiadlxx.dll";

        private static object ApplyDelegate(string functionName, FieldInfo field){
            var attribute = new DllImportAttribute(DllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = functionName
            };

            Native.CreatePInvokeDelegate(attribute, field.FieldType, out var delegateInstance);
            return delegateInstance;
        }

        public static void Shutdown(){
            if (_isInitialized) {
                try {
                    AdlMainControlDestroy();
                } catch {
                    ;
                }
            }
        }
    }
}
