﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            var targetType = assembly.GetExportedTypes().FirstOrDefault(type => type.BaseType == typeof(BaseModule));
            if (targetType == null) {
                throw new ModuleLoadingException("Assembly doesn't contains BaseModule class instance");
            }

            dynamic obj;
            try {
                obj = Activator.CreateInstance(targetType, this);
            } catch (Exception err) {
                throw new ModuleLoadingException("Unnable to create BaseModule class instance", err);
            }

            return (BaseModule)obj;
        }

        public abstract bool UnloadModule(BaseModule module);

        public abstract void UnloadAllModules();

        public BaseCollectingServer GetServer(){
            return _server;
        }
    }
}