﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.TemperatureApi;
using server.Utils;

namespace server
{
    public class Program
    {
        public static Logger AppLogger;

        static void Main(string[] args){
            AppLogger = new Logger();
            AppLogger.Initialize();

            var loader = new TemperatureModuleLoader();
            loader.LoadFromFolder(@"..\..\..\modules\temperature\cpu_shared\output\win32");

            Console.WriteLine("Load done");
            Console.ReadLine();

            AppLogger.Shutdown();
        }
    }
}
