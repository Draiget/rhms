using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using server.Addons;
using server.Hardware;
using server.Networking;
using server.Utils.Logging;

namespace server.Modules.Base
{
    public abstract class BaseCollectingServer : ICollectingServer
    {
        public const string SettingsFileName = "server-settings.json";
        
        public abstract InfluxDbConnection GetInfluxDbConnection();

        protected RhmsSettings Settings;
        protected NetworkConnectionManager Manager;

        public bool LoadSettings(){
            if (!File.Exists(SettingsFileName)) {
                Logger.Info($"Server settings file '{SettingsFileName}' not found, creating a default one");
                Settings = RhmsSettings.Default;
                SaveSettings();
                return true;
            }

            try {
                var conv = JsonConvert.DeserializeObject<RhmsSettings>(File.ReadAllText(SettingsFileName));
                Settings = conv;
                return true;
            }
            catch(Exception err) {
                Logger.Error($"Unable to load settings from file '{SettingsFileName}'", err);
                return false;
            }
        }

        public abstract bool Initialize();

        public abstract void StartHardwareUpdates();

        public abstract void StartNetworkUpdates();

        protected bool SaveSettings(){
            try {
                var conv = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(SettingsFileName, conv);
                return true;
            } catch (Exception err) {
                Logger.Error($"Unable to serialize (save) settings to file '{SettingsFileName}'", err);
                return false;
            }
        }

        public RhmsSettings GetSettings(){
            return Settings;
        }

        public bool HasActiveRemoteConnection(){
            throw new NotImplementedException();
        }

        public abstract bool IsKernelDriverAvailable();

        public abstract BaseModuleLoader GetModuleLoader();

        public void Shutdown() {
            try {
                OnShutdown();
            } catch {
                ;
            }

            try {
                GetModuleLoader().UnloadAllModules();
            } catch {
                ;
            }

            SaveSettings();
        }

        public void RegisterHardware(IHardware hardware) {
            throw new NotImplementedException();
        }

        public NetworkConnectionManager GetNetConnectionManager() {
            return Manager;
        }

        public abstract void OnShutdown();
    }
}
