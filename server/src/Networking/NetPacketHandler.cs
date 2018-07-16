using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Drivers;
using server.Networking.Control.Packets;
using server.Utils;
using server.Utils.Logging;

namespace server.Networking
{
    public class NetPacketHandler
    {
        private readonly RhmsCollectingServer _collectingServer;

        public NetPacketHandler(RhmsCollectingServer server, NetworkConnectionManager connectionManager, NetworkClient netClient) {
            _collectingServer = server;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void ProcessRebootPacket(PacketCReboot packet) {
            Logger.Info("Reboot packet received!");
            Logger.Info("Trying native shutdown ...");
            try {
                Native.RestartComputerApi();
            } catch (Exception ex) {
                Logger.Error("Unable to shutdown Windows wia DoExitWin!", ex);
                Logger.Info("Trying DOS shutdown command ...");
                try {
                    Native.RestartComputerCommand();
                } catch (Exception ex2) {
                    Logger.Error("Unable to shutdown Windows wia DOS command!", ex2);
                }
            }

            /*
            Logger.Info("Killing all miner processes ...");
            // Kill miners apps
            try {
                var ps = Process.GetProcessesByName("miner");
                foreach (var process in ps) {
                    try {
                        process.Kill();
                    } catch (Exception e) {
                        Logger.Error("Unable to restart (by killing) GPU expensive process", e);
                    }
                }
            } catch (Exception e) {
                Logger.Error("Unable to obtain GPU expensive processes", e);
            }

            Logger.Info("Reboot packet received! Disabling display drivers ...");
            var result = KernelDriverBridge.StopDsplayDriver();
            if (result != KernelDriverInitState.RhmsDriverManagerDisplayDriverDisableOk) {
                Logger.Warn($"Disabling driver result is not ok: {result.ToString()}");
                return;
            }

            Logger.Info("Waiting 2 seconds ...");
            Thread.Sleep(2000);
            Logger.Info("Starting display driver ...");

            result = KernelDriverBridge.StartDsplayDriver();
            if (result != KernelDriverInitState.RhmsDriverManagerDisplayDriverEnableOk) {
                Logger.Warn($"Enabling driver result is not ok: {result.ToString()}");
                return;
            }

            Logger.Info("Killing all miner processes ...");
            // Kill miners apps
            try {
                var ps = Process.GetProcessesByName("miner");
                foreach (var process in ps) {
                    try {
                        process.Kill();
                    } catch (Exception e) {
                        Logger.Error("Unable to restart (by killing) GPU expensive process", e);
                    }
                }
            } catch (Exception e) {
                Logger.Error("Unable to obtain GPU expensive processes", e);
            }

            Logger.Info("Display driver is successfully restarted");*/
        }

        public void ProcessPingPacket(PacketCPing packetCPing) {
            
        }
    }
}
