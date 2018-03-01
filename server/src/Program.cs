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
using server.Utils.Logging;

namespace server
{
    public class Program
    {
        // bcdedit.exe /set loadoptions DDISABLE_INTEGRITY_CHECKS
        // bcdedit.exe /set testsigning ON

        // GPEdit.msc ~ User Configuration -> Administrative Templates -> System -> Driver Installation -> Code signing for drivers ~ Disabled -> OK

        public static Logger AppLogger;

        internal static void Main(string[] args) {
            Console.Title = "RHMS Client Data Collecting Server";
            Thread.CurrentThread.Name = "main";

            AppLogger = new Logger();
            AppLogger.Initialize();
            Logger.Info("Starting server ...");
            Logger.Info($"Working directory '{Directory.GetCurrentDirectory()}'");

            BridgeDriver.RegisterLoggerCallback(DriverLoggerCallback);

            var state = KernelDriverBridge.InitializeEnvironment();
            if (state == KernelDriverInitState.RhmsDrvNoError) {
                var cpuInfo = Drivers.Presentation.Cpuid.CollectAll();
                if (cpuInfo != null) {
                    Console.WriteLine(cpuInfo.ToString());
                    Console.WriteLine($"Core loads: {cpuInfo.GetCpuLoad()}");
                }
            }

            Logger.Auto(state == KernelDriverInitState.RhmsDrvNoError ? BridgeDriver.LogLevel.Info : BridgeDriver.LogLevel.Error,
                        $"Load driver state: {state}");

            var collectingServer = new RhmsCollectingServer();
            Debug.Assert(collectingServer.GetModuleLoader() != null);

            Logger.Info("Loading modules ...");
            collectingServer.GetModuleLoader().LoadFromFolder(Directory.GetCurrentDirectory() + @"\modules");
            Logger.Info("Loading modules has finished.");

            Console.ReadLine();

            // Do not remove service if we are create one
            KernelDriverBridge.DeinitializeEnvironment();
            AppLogger.Shutdown();
        }

        private static void DriverLoggerCallback(BridgeDriver.LogLevel level, string message){
            Logger.Auto(level, message);
        }
    }
}
