using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using server.Utils.Logging;
using server.Utils.Natives;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local

namespace server.Utils
{
    public static class Native
    {
        private static readonly ModuleBuilder _moduleBuilder =
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("PInvokeDelegateFactoryInternalAssembly"), AssemblyBuilderAccess.Run)
            .DefineDynamicModule("PInvokeDelegateFactoryInternalModule");

        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int NtQuerySystemInformation(
            SystemInformationClass informationClass,
            [Out] SSystemProcessorPerformanceInformation[] informations,
            int structSize,
            out IntPtr returnLength);

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
            public readonly ushort wServicePackMajor;
            public readonly ushort wServicePackMinor;
            public readonly ushort wSuiteMask;
            public readonly byte wProductType;
            public readonly byte wReserved;
        }

        public const int VER_PLATFORM_WIN32s = 0;
        public const int VER_PLATFORM_WIN32_WINDOWS = 1;
        public const int VER_PLATFORM_WIN32_NT = 2;

        private static readonly IDictionary<Tuple<DllImportAttribute, Type>, Type> _wrapperTypes =
            new Dictionary<Tuple<DllImportAttribute, Type>, Type>();

        public static void CreatePInvokeDelegate<T>(DllImportAttribute attr, out T outDelegate) where T : class {
            var key = new Tuple<DllImportAttribute, Type>(attr, typeof(T));
            _wrapperTypes.TryGetValue(key, out var wrapperType);

            if (wrapperType == null) {
                wrapperType = CreatePInvokeWrapper(typeof(T), attr);
                _wrapperTypes.Add(key, wrapperType);
            }

            outDelegate = Delegate.CreateDelegate(typeof(T), wrapperType, attr.EntryPoint) as T;
        }

        public static void CreatePInvokeDelegate(DllImportAttribute attr, Type delegateType, out Delegate outDelegate) {
            var key = new Tuple<DllImportAttribute, Type>(attr, delegateType);
            _wrapperTypes.TryGetValue(key, out var wrapperType);

            if (wrapperType == null) {
                wrapperType = CreatePInvokeWrapper(delegateType, attr);
                _wrapperTypes.Add(key, wrapperType);
            }

            outDelegate = Delegate.CreateDelegate(delegateType, wrapperType, attr.EntryPoint);
        }

        private static Type CreatePInvokeWrapper(Type delegateType, DllImportAttribute attr){
            var typeBuilder = _moduleBuilder.DefineType("PInvokeDelegateFactoryInternalWrapperType" + _wrapperTypes.Count);

            var methodInfo = delegateType.GetMethod("Invoke");
            if (methodInfo == null) {
                return null;
            }

            var parameterInfos = methodInfo.GetParameters();
            var parameterCount = parameterInfos.GetLength(0);

            var parameterTypes = new Type[parameterCount];
            for (var i = 0; i < parameterCount; i++) {
                parameterTypes[i] = parameterInfos[i].ParameterType;
            }

            var methodBuilder = typeBuilder.DefinePInvokeMethod(
                attr.EntryPoint, 
                attr.Value,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl, CallingConventions.Standard,
                methodInfo.ReturnType, 
                parameterTypes,
                attr.CallingConvention,
                attr.CharSet);

            foreach (ParameterInfo parameterInfo in parameterInfos) {
                methodBuilder.DefineParameter(parameterInfo.Position + 1, parameterInfo.Attributes, parameterInfo.Name);
            }

            if (attr.PreserveSig) {
                methodBuilder.SetImplementationFlags(MethodImplAttributes.PreserveSig);
            }

            return typeBuilder.CreateType();
        }

        public static bool IfLibraryIsExists(string dllName) {
            try {
                var code = LoadLibrary(dllName);
                return code != 0;
            } catch (DllNotFoundException) {
                return false;
            } catch (BadImageFormatException) {
                return true;
            } catch (Exception e) {
                Logger.Error("Library exists check unexpected error", e);
                return false;
            }
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
    }
}
