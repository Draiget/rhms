using System.Runtime.InteropServices;

namespace server.Modules.TemperatureApi
{
    [StructLayout(LayoutKind.Sequential)]
    public class BaseTemperatureModule
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string ModuleName;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [ApiCallRenaming("module_open")]
        internal delegate bool Open(uint apiVersion, out SModuleState state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] 
        [ApiCallRenaming("module_close")]
        internal delegate bool Close(out SModuleState state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [ApiCallRenaming("module_get_instance")]
        internal delegate bool GetInstance([Out] BaseTemperatureModule module);
    }
    
}
