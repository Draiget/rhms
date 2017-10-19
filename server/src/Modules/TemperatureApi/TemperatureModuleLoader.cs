using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace server.Modules.TemperatureApi
{
    public class TemperatureModuleLoader : IModueLoader
    {
        private readonly List<NativeModuleHolder> _loadedModules;

        public TemperatureModuleLoader(){
            _loadedModules = new List<NativeModuleHolder>();
        }

        public void UnloadAll() {
            foreach (var moduleHolder in _loadedModules) {
                CloseModule(moduleHolder);
            }
        }

        public void LoadFromFolder(string folderName){
            foreach (var dynamicLibPath in Directory.GetFiles(folderName, "*.dll")) {
                var handle = Native.LoadLibrary(dynamicLibPath);
                if (handle <= 0) {
                    continue;
                }

                var openRename = GetApiRenameFromDelegate(typeof(BaseTemperatureModule.Open)).ProcedureName;
                var openProc = Marshal.GetDelegateForFunctionPointer<BaseTemperatureModule.Open>(Native.GetProcAddress(handle, openRename));
                if (openProc == null) {
                    // Bad module?
                    // TODO: Debug call
                    Native.FreeLibrary(handle);
                    continue;
                }

                var instanceRename = GetApiRenameFromDelegate(typeof(BaseTemperatureModule.GetInstance)).ProcedureName;
                var instanceProc = Marshal.GetDelegateForFunctionPointer<BaseTemperatureModule.GetInstance>(Native.GetProcAddress(handle, instanceRename));
                if (instanceProc == null) {
                    // Bad module?
                    // TODO: Debug call
                    Native.FreeLibrary(handle);
                    continue;
                }

                OpenModule(openProc, instanceProc, handle);
            }
        }

        private void OpenModule(BaseTemperatureModule.Open openCallback, BaseTemperatureModule.GetInstance instanceCallback, int handle){
            try {
                var isOk = openCallback(1, out var state);
                if (!isOk) {
                    if (state.Code != 1) {
                        // Console.WriteLine($"OpenModule error [{state}]: {state.Message}");
                        // TODO: Debug call
                    }

                    Native.FreeLibrary(handle);
                    handle = -1;
                    return;
                }

                var module = new BaseTemperatureModule();
                var isInstanceOk = instanceCallback(module);
                if (!isInstanceOk) {
                    // Console.WriteLine($"OpenModule error [{state}]: {state.Message}");
                    // TODO: Debug call

                    Native.FreeLibrary(handle);
                    handle = -1;
                    return;
                }

                _loadedModules.Add(new NativeModuleHolder(handle, null));
            } catch (Exception) {
                if (handle > 0) {
                    Native.FreeLibrary(handle);
                }
            }
        }

        private static void CloseModule(NativeModuleHolder holder) {
            try {
                if (holder.DllHandle > 0) {
                    Native.FreeLibrary(holder.DllHandle);
                }
            } catch (Exception) {
                ;
            }
        }

        internal ApiCallRenamingAttribute GetApiRenameFromDelegate(Type delegateType){
            Debug.Assert(delegateType != null, "Cannot find BaseTemperatureModule.Open method");

            var attribs = delegateType.GetCustomAttributes(typeof(ApiCallRenamingAttribute), false);
            Debug.Assert(attribs.Length > 0, "BaseTemperatureModule.Open doesn't contains api call remap attribute");

            var apiRenaming = (ApiCallRenamingAttribute)attribs[0];
            Debug.Assert(apiRenaming != null, "BaseTemperatureModule.Open: Null attribute data!");

            return apiRenaming;
        }

        private struct NativeModuleHolder
        {
            public readonly int DllHandle;
            public readonly BaseTemperatureModule Module;

            public NativeModuleHolder(int handle, BaseTemperatureModule module){
                DllHandle = handle;
                Module = module;
            }
        }
    }
}
