using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Delegates;
using gpu_radeon.Api.Enums;
using gpu_radeon.Api.Helpers;
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
        public static AdlApiGpu.ADL_Adapter_AdapterInfo_GetDelegate AldAdapterGetInfo;

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

        private static void GetDelegate<T>(string entryPoint, out T newDelegate) where T : class {
            var attribute = new DllImportAttribute(DllName) {
                CallingConvention = CallingConvention.Cdecl,
                PreserveSig = true,
                EntryPoint = entryPoint
            };

            Native.CreatePInvokeDelegate(attribute, out newDelegate);
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
