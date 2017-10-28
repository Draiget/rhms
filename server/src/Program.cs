using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Drivers;
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

            var state = KernelDriverBridge.InitializeEnvironment();

            Console.WriteLine($"Load driver state: {state}");
            Console.ReadLine();

            AppLogger.Shutdown();
        }
    }
}
