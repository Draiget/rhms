#undef Rhms_DebugSensors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Addons;
using server.Modules;
using server.Modules.Base;
using server.Modules.Extended;
using server.Networking;
using server.Utils;
using server.Utils.Logging;

namespace server
{
    public class RhmsCollectingServer : BaseCollectingServer
    {
        private readonly BaseModuleLoader _moduleLoader;
        private InfluxDbConnection _influxConnection;

        private bool _isThreadsActive;
        private readonly Thread _hardwareUpdaterThread;

        public RhmsCollectingServer(){
            _moduleLoader = new ServerModuleLoader(this);
            _hardwareUpdaterThread = new Thread(WorkerHardwareUpdater);
        }

        public override InfluxDbConnection GetInfluxDbConnection(){
            return _influxConnection;
        }

        public override bool Initialize() {
            if (Settings.InfluxOutput.Enabled) {
                _influxConnection = new InfluxDbConnection(Settings.InfluxOutput.Host, Settings.InfluxOutput.Database, Settings.InfluxOutput.Retention);
            }

            // Temporary here
            Manager = new NetworkConnectionManager();
            Manager.Initialize();

            _isThreadsActive = true;
            return true;
        }

        public override BaseModuleLoader GetModuleLoader(){
            return _moduleLoader;
        }

        public override void OnShutdown() {
            _isThreadsActive = false;
            ThreadUtils.JoinIgnoreErrors(_hardwareUpdaterThread);
        }

        public override void StartHardwareUpdates() {
            _hardwareUpdaterThread.Name = "hardware_updater";
            _hardwareUpdaterThread.Start();
        }

        private void WorkerHardwareUpdater() {
            var loadedModules = _moduleLoader.GetHardwareModules();
            while (_isThreadsActive) {
                foreach (var module in loadedModules) {
                    foreach (var hardware in module.GetHardware()) {
                        hardware.TickUpdate();

                        #if Rhms_DebugSensors
                        foreach (var sensor in hardware.GetSensors()) {
                            Logger.Info($"[{hardware.Identify().GetFullSystemName()}] {sensor.GetDisplayName()}: {sensor.GetValue()}");
                        }
                        #endif
                    }
                }

                foreach (var module in loadedModules) {
                    try {
                        module.AfterHardwareTick();
                    } catch (Exception e) {
                        Logger.Error($"Error after hardware tick in module [{module.GetLogIdentifer()}] '{module.GetName()}'", e);
                    }
                }

                Thread.Sleep(Settings.HardwareSensorsUpdateInterval);
            }
        }

        public override void StartNetworkUpdates() {
            throw new NotImplementedException();
        }
    }
}
