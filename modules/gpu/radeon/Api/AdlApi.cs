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
        private static AdlApiGpu.ADL_Adapter_AdapterInfo_GetDelegate _aldAdapterGetInfo;

        public const int AdlMaxPath = 256;

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
                    from typeField in assemblyType.GetFields()
                    let fieldAttribute = typeField.GetCustomAttribute<LinkedFieldAttribute>()
                    where fieldAttribute != null
                    select typeField);
            }

            // Search target field for each delegate attributes
            foreach (var delegateContainerClass in delegatesList) {
                var attr = delegateContainerClass.GetCustomAttribute<LinkedDelegateAttribute>();
                if (attr == null) {
                    continue;
                }

                var hasLinkedField = false;

                foreach (var field in fieldsList) {
                    var linkedAttr = field.GetCustomAttribute<LinkedFieldAttribute>();
                    if (linkedAttr?.TargetLookType == delegateContainerClass.ReflectedType) {
                        hasLinkedField = true;

                        try {
                            field.SetValue(null, ApplyDelegate(attr.FunctionName, field));
                            ModuleGpuRadeon.Logger.Debug($"Linked delegate field '{field.Name}' with function '{delegateContainerClass}' ({attr.FunctionName})");
                        } catch (FieldAccessException e) {
                            ModuleGpuRadeon.Logger.Error($"Failed to linke delegate field '{field.Name}' with function id '{delegateContainerClass}' ({attr.FunctionName})", e);
                        }
                    }
                }

                if (!hasLinkedField) {
                    ModuleGpuRadeon.Logger.Warn($"Delegate with function '{delegateContainerClass}' is not linked (not using)");
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
