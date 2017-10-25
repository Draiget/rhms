using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local

namespace server
{
    internal class Native
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        internal static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        internal static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        internal static extern bool FreeLibrary(int hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);

        [StructLayout(LayoutKind.Sequential)]
        public struct OSVERSIONINFOEX
        {
            public readonly uint dwOSVersionInfoSize;
            public readonly uint dwMajorVersion;
            public readonly uint dwMinorVersion;
            public readonly uint dwBuildNumber;
            public readonly uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string szCSDVersion;
            public readonly UInt16 wServicePackMajor;
            public readonly UInt16 wServicePackMinor;
            public readonly UInt16 wSuiteMask;
            public readonly byte wProductType;
            public readonly byte wReserved;
        }

        public const int VER_PLATFORM_WIN32s = 0;
        public const int VER_PLATFORM_WIN32_WINDOWS = 1;
        public const int VER_PLATFORM_WIN32_NT = 2;
    }
}
