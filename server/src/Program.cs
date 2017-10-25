using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.Drivers;
using server.Modules.TemperatureApi;
using server.Utils;
using server.Utils.Logging;

namespace server
{
    public class Program
    {
        public static Logger AppLogger;

        static void Main(string[] args){
            Thread.CurrentThread.Name = "main";

            AppLogger = new Logger();
            AppLogger.Initialize();

            /*var loader = new TemperatureModuleLoader();
            loader.LoadFromFolder(@"..\..\..\modules\temperature\cpu_shared\output\win32");*/
            var state = KernelDriverBridge.InitializeEnvironment();

            Console.WriteLine($"Load driver state: {state}");
            Console.ReadLine();

            AppLogger.Shutdown();
        }
    }
}
