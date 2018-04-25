using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Drivers;
using server.Drivers.Kernel;
using server.Modules;
using server.Utils;
using server.Utils.Logging;

namespace server
{
    public class Program
    {
        // bcdedit.exe /set loadoptions DDISABLE_INTEGRITY_CHECKS
        // bcdedit.exe /set testsigning ON

        // GPEdit.msc ~ User Configuration -> Administrative Templates -> System -> Driver Installation -> Code signing for drivers ~ Disabled -> OK

        private static bool _isCanShutdown;
        public static Logger AppLogger;
        public static RhmsCollectingServer CollectingServer;

        internal static void Main(string[] args) {
            _isCanShutdown = false;
            Native.SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

            Console.Title = "RHMS Client Data Collecting Server";
            Thread.CurrentThread.Name = "main";

            AppLogger = new Logger();
            AppLogger.Initialize();
            Logger.Info("Starting server");
            Logger.Info($"Working directory '{Directory.GetCurrentDirectory()}'");

            BridgeDriver.RegisterLoggerCallback(DriverLoggerCallback);

            var state = KernelDriverBridge.InitializeEnvironment();
            if (state == KernelDriverInitState.RhmsDrvNoError) {
                var cpusInfo = Drivers.Presentation.Cpuid.Get();
                var firstCpu = cpusInfo?[0];
                if (firstCpu != null) {
                    Console.WriteLine(firstCpu.ToString());
                }
            }

            Logger.Auto(state == KernelDriverInitState.RhmsDrvNoError ? DriverLogLevel.Info : DriverLogLevel.Error,
                        $"Load driver state: {state}");

            if (state != KernelDriverInitState.RhmsDrvNoError) {
                if (state == KernelDriverInitState.RhmsDriverManagerIncorrectDrvSignature) {
                    Logger.Warn("Kernel driver signature checking is enabled in your system, RHMS server is not able to load custom kernel driver.");
                    Logger.Warn("You need to disable signature checking on target subsystem.");
                } else {
                    Logger.Warn("Kernel driver are failed to initializate, maybe driver signing checks are enabled on target system.");
                }

                Logger.Warn("Some features as collecting physical devices data will not available.");
            }

            CollectingServer = new RhmsCollectingServer();
            Debug.Assert(CollectingServer.GetModuleLoader() != null);

            if (!CollectingServer.LoadSettings()) {
                Logger.Warn("Unable to load server settings!");
                AppLogger.Shutdown();
                return;
            }

            Logger.Info("Initialize collecting server");
            CollectingServer.Initialize();

            Logger.Info("Loading modules");
            CollectingServer.GetModuleLoader().LoadFromFolder(Directory.GetCurrentDirectory() + @"\modules");
            Logger.Info("Loading modules has finished.");

            CollectingServer.StartHardwareUpdates();
            _isCanShutdown = true;

            // There's no commands yet, just listen for any line and close
            Console.ReadLine();

            Shutdown();
        }

        private static void Shutdown(bool externSignal = false) {
            if (externSignal) {
                Logger.Info("Receive shutdown event");
            }

            while (!_isCanShutdown) {
                Thread.Sleep(100);
            }

            Logger.Info("Shutting down server");
            CollectingServer?.Shutdown();

            // Do not remove service if we are create one
            KernelDriverBridge.DeinitializeEnvironment();
            AppLogger?.Shutdown();
        }

        private static void DriverLoggerCallback(DriverLogLevel level, string message){
            Logger.Auto(level, message);
        }

        private static bool ConsoleCtrlCheck(Native.CtrlTypes type) {
            Shutdown(true);
            return true;
        }
    }
}
