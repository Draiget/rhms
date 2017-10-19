using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.TemperatureApi;

namespace server
{
    public class Program
    {
        static void Main(string[] args){
            var loader = new TemperatureModuleLoader();
            loader.LoadFromFolder(@"C:\rhms\modules\temperature\cpu_shared\output\x86");

            Console.WriteLine("Load done");
            Console.ReadLine();
        }
    }
}
