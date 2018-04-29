using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using server.Settings;
using server.Utils.Logging;

namespace server.Modules.Base
{
    public abstract class BaseModuleLoader : IModuleLoader
    {
        private readonly BaseCollectingServer _server;
        private readonly List<BaseModule> _modules;

        protected BaseModuleLoader(BaseCollectingServer server){
            _server = server;
            _modules = new List<BaseModule>();

            CreateDirectories();
        }

        private void CreateDirectories(){
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\modules");
        }

        public void LoadFromFolder(string folderName) {
            if (_modules.Count > 0) {
                Logger.Warn("Calling loading modules from folder without unloading");
                UnloadAllModules();
            }

            string[] files;
            try {
                files = Directory.GetFiles(folderName, "*.dll");
            }
            catch (Exception ex) {
                Logger.Error($"Unnable to get files from '{folderName}'", ex);
                return;
            }

            foreach (var dllFile in files) {
                BaseModule module;
                try {
                    module = LoadFromFile(dllFile);
                    Logger.Info($"Loaded '{module.GetName()}' [{module.GetLogIdentifer()}]");
                } catch (ModuleLoadingException err) {
                    Logger.Error($"Unnable to load dll module file {dllFile}: {err.Message}", err.InnerException);
                    continue;
                }

                GetSettingsFor(module);

                if (!module.CheckForSystemSupport()) {
                    Logger.Warn($"Unable to load '{module.GetName()}': {module.GetUnsupportedReason()} [Module is not supported]");
                    continue;
                }

                if (!module.Open()) {
                    Logger.Warn($"Failed to load '{module.GetName()}': Open fails");
                    continue;
                }

                Logger.Info($"Module '{module.GetName()}' is loaded - OK");
                _modules.Add(module);
            }
        }

        public BaseModule LoadFromFile(string filePath) {
            Assembly assembly;
            try {
                assembly = Assembly.LoadFile(filePath);
            } catch (BadImageFormatException err) {
                throw new ModuleLoadingException("Bad image format", err);
            } catch (FileNotFoundException err) {
                throw new ModuleLoadingException("File not found by I/O", err);
            } catch (FileLoadException err) {
                throw new ModuleLoadingException("Assembly can't be loaded", err);
            } catch (Exception ex) {
                throw new ModuleLoadingException("Unknown exception thrown", ex);
            }

            try {
                var targetType = assembly.GetExportedTypes().FirstOrDefault(type => type.BaseType == typeof(BaseHardwareModule));
                if (targetType == null) {
                    // Try to load as base module instead
                    targetType = assembly.GetExportedTypes().FirstOrDefault(type => type.BaseType == typeof(BaseModule));
                    if (targetType == null) {
                        throw new ModuleLoadingException($"Assembly doesn't contains {nameof(BaseHardwareModule)} or {nameof(BaseModule)} class instance");
                    }
                }

                dynamic obj;
                try {
                    obj = Activator.CreateInstance(targetType, this);
                }
                catch (Exception err) {
                    throw new ModuleLoadingException("Unnable to create BaseModule class instance", err);
                }

                return (BaseModule) obj;
            }
            catch (TypeLoadException err) {
                throw new ModuleLoadingException($"Target module doesn't propertly release API for target type '{err.TypeName}'", err);
            }
        }

        public abstract bool UnloadModule(BaseModule module);

        public abstract void UnloadAllModules();

        public BaseCollectingServer GetServer(){
            return _server;
        }

        public List<BaseModule> GetAllModules() {
            return _modules;
        }

        public List<BaseHardwareModule> GetHardwareModules() {
            return _modules.Where(x => x is BaseHardwareModule).Cast<BaseHardwareModule>().ToList();
        }

        public RhmsSettings GetSettings() {
            return _server.GetSettings();
        }

        public BaseModuleSettings GetSettingsFor(BaseModule baseModule) {
            var typeObj = baseModule.GetSettingsType();
            if (typeObj == null) {
                return null;
            }

            BaseModuleSettings typeInstance;
            try {
                var type = (TypeInfo) typeObj;
                typeInstance = (BaseModuleSettings) Activator.CreateInstance(type, baseModule);
                typeInstance.ApplyModule(baseModule);
            } catch (Exception e) {
                Logger.Error($"Unable to load settings for module {baseModule.GetName()} [id={baseModule.GetLogIdentifer()}], can not create instance of settings class '{typeObj}", e);
                return null;
            }

            var result = SettingsManager.LoadFromDisk(ref typeInstance);
            if (result.HasNoErrors()) {
                typeInstance.GenerateDefault();
                return typeInstance;
            }

            Logger.Error(
                $"Unable to load settings for module {baseModule.GetName()} [id={baseModule.GetLogIdentifer()}]",
                result.Error == null ? new Exception(result.ErrorMessage) : result.Error);

            return null;
        }

        public SettingsIoResult? SaveSettingsFor(BaseModule baseModule) {
            return SaveSettingsForInternal(baseModule, baseModule.SettingsInstance);
        }

        public SettingsIoResult? SaveSettingsFor(BaseModule baseModule, BaseModuleSettings settings) {
            return SaveSettingsForInternal(baseModule, settings, true);
        }

        private SettingsIoResult? SaveSettingsForInternal(BaseModule baseModule, BaseModuleSettings settings, bool overrideDef = false) {
            if (baseModule == null) {
                Logger.Error("Trying to save settings for module that are null");
                return null;
            }

            if (overrideDef) {
                baseModule.SettingsInstance = settings;
            }

            var result = SettingsManager.UpdateOrCreate(baseModule.SettingsInstance);
            if (result.HasNoErrors()) {
                return result;
            }

            Logger.Error(
                $"Unable to save settings for module {baseModule.GetName()} [id={baseModule.GetLogIdentifer()}]",
                result.Error == null ? new Exception(result.ErrorMessage) : result.Error);

            return result;
        }
    }
}
