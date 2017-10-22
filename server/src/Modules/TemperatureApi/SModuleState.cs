using System.Runtime.InteropServices;

namespace server.Modules.TemperatureApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SModuleState
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        private readonly string _errorString;

        public SModuleState(int errorCode = 1, string errString = ""){
            Code = errorCode;
            _errorString = errString;
        }

        public int Code {
            get;
        }

        public string Message => _errorString;
    }
}
