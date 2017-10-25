using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers
{
    public static class KernelDriverBridge
    {
        private static KernelDriverInitState _initState;

        private const string DriverName = "rhms_bridge";

        public static KernelDriverInitState InitializeEnvironment(){
            _initState = ApiInitializeDriver();
            if (_initState == KernelDriverInitState.RhmsDrvNoError) {
                Test();
            }

            return _initState;
        }

        public static void Test(){
            uint index = 0, eax = 0, ebx = 0, ecx = 0, edx = 0;
            var str = string.Empty;

            index = 0x00000010; // Time Stamp Counter
            if (DriverReadMsrTx(index, ref eax, ref edx, (UIntPtr)1) != 0) {
                str += "index     63-32    31-0\r\n";
                str += index.ToString("X8") + ": " + edx.ToString("X8")
                       + " " + eax.ToString("X8") + "\r\n";
            } else {
                str += "Failure : Change Thread Affinity Mask\r\n";
            }

            Console.WriteLine(str);
        }

        private const string DriverFullPath = "bin\\" + DriverName + ".dll";

        [DllImport(DriverFullPath, EntryPoint = "RHMS_InitializeDriver", CallingConvention = CallingConvention.Cdecl)]
        private static extern KernelDriverInitState ApiInitializeDriver();

        [DllImport(DriverFullPath, EntryPoint = "RHMS_ReadMsr", CallingConvention = CallingConvention.Winapi)]
        private static extern int DriverReadMsr(uint index, ref uint eax, ref uint edx);

        [DllImport(DriverFullPath, EntryPoint = "RHMS_ReadMsrTx", CallingConvention = CallingConvention.Winapi)]
        private static extern int DriverReadMsrTx(uint index, ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        [DllImport(DriverFullPath, EntryPoint = "RHMS_ReadMsrPx", CallingConvention = CallingConvention.Winapi)]
        private static extern int DriverReadMsrPx(uint index, ref uint eax, ref uint edx, UIntPtr processAffinityMask);
    }
}
