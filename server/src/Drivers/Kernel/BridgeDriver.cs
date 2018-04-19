using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    internal static class BridgeDriver
    {
        /// <summary>
        /// Checking if current windows is based on NT version
        /// </summary>
        /// <returns>Supports or not</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_IsNT", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsNt();

        /// <summary>
        /// Checking is installed CPU supports CPUID
        /// </summary>
        /// <returns>Supports or not</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_IsCPUIDSupported", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsCpuid();

        /// <summary>
        /// Checking if system supports MSR (Model-Specific Registers)
        /// </summary>
        /// <returns>Supports or not</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_IsMSRSupported", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsMsr();

        /// <summary>
        /// Checking if system supports TSC (Time Stamp Counter)
        /// </summary>
        /// <returns>Supports or not</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_IsTSCSupported", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsTsc();

        /// <summary>
        /// Determines whether the specified process is running under WOW64
        /// </summary>
        /// <returns>Is running under WOW64 or not</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_IsWow64", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsWow64();

        /// <summary>
        /// Determines installed processor architecture is x64
        /// </summary>
        /// <returns>Is processor architecture is x64</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_IsArch64", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsArch64();

        /// <summary>
        /// Initializing driver (install service if not installed and run it)
        /// </summary>
        /// <returns>Initialization status enum (KernelDriverInitState)</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_InitializeDriver", CallingConvention = CallingConvention.Winapi)]
        internal static extern KernelDriverInitState Initialize();

        /// <summary>
        /// Initializing driver (stopping driver service)
        /// </summary>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_DeinitializeDriver", CallingConvention = CallingConvention.Winapi)]
        internal static extern void Deinitialize();


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void LoggerCallback([MarshalAs(UnmanagedType.I4)] DriverLogLevel level, [MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_RegisterLoggerCallback", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool RegisterLoggerCallback(LoggerCallback callback);
    }

    public enum DriverLogLevel
    {
        Debug = 0,
        Error,
        Warning,
        Info
    }
}
