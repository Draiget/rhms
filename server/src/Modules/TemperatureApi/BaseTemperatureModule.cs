using System.Runtime.InteropServices;

namespace server.Modules.TemperatureApi
{
    [StructLayout(LayoutKind.Sequential)]
    public class BaseTemperatureModule
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        private string _moduleName;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [ApiCallRenaming("module_open")]
        internal delegate bool Open(uint apiVersion, out SModuleState state, ref BaseTemperatureModule module);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] 
        [ApiCallRenaming("module_close")]
        internal delegate bool Close(out SModuleState state);
    }
    
}
