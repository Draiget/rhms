using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.Drivers
{
    public static class KernelDriverBridge
    {
        private static KernelDriverInitState _initState;

        private const string DriverName = "rhms_bridge";

        public static KernelDriverInitState InitializeEnvironment(){
            _initState = Kernel.BridgeDriver.Initialize();
            if (_initState == KernelDriverInitState.RhmsDrvNoError) {
                Test();
            }

            return _initState;
        }

        private const uint Ia32ThermStatusMsr = 0x019C;
        private const uint Ia32TemperatureTarget = 0x01A2;

        private static float GetTjMaxFromMsr(){
            uint eax = 0, edx = 0;
            if (Kernel.Msr.ReadTx(Ia32TemperatureTarget, ref eax, ref edx, (UIntPtr)1)) {
                return (eax >> 16) & 0xFF;
            }

            return 100;
        }

        public static void Test(){
            uint eax = 0, edx = 0;

            while (true) {
                if (Kernel.Msr.ReadTx(Ia32ThermStatusMsr, ref eax, ref edx, (UIntPtr) 1) && (eax & 0x80000000) != 0) {
                    // get the dist from tjMax from bits 22:16
                    float deltaT = ((eax & 0x007F0000) >> 16);
                    var tjMax = GetTjMaxFromMsr();
                    var tSlope = 1;
                    var temp = tjMax - tSlope * deltaT;
                    Console.Write($"\rTemperature: {temp}");
                    Thread.Sleep(700);
                }
            }
        }

        internal const string DriverFullPath = "bin\\" + DriverName + ".dll";

    }
}
