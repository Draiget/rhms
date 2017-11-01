using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Drivers;
using server.Drivers.Kernel;
using server.Utils;
using server.Utils.Logging;

namespace server
{
    public class Program
    {
        public static Logger AppLogger;

        internal static void Main(string[] args) {
            Console.Title = "RHMS Client Data Collecting Server";
            Thread.CurrentThread.Name = "main";

            AppLogger = new Logger();
            AppLogger.Initialize();
            Logger.Info("Starting server ...");

            BridgeDriver.RegisterLoggerCallback(Target);

            var state = KernelDriverBridge.InitializeEnvironment();
            if (state == KernelDriverInitState.RhmsDrvNoError) {
                var cpuInfo = Drivers.Presentation.Cpuid.CollectAll();
                if (cpuInfo != null) {
                    Console.WriteLine(cpuInfo.ToString());
                }
            }

            Console.WriteLine($"Load driver state: {state}");
            Console.ReadLine();

            AppLogger.Shutdown();
        }

        private static void Target(BridgeDriver.LogLevel level, string message){
            Logger.Auto(level, message);
        }
    }
}
