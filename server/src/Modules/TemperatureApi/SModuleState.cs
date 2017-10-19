using System.Runtime.InteropServices;

namespace server.Modules.TemperatureApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SModuleState
    {
        private readonly int _errorCode;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        private readonly string _errorString;

        public SModuleState(int errorCode = 1, string errString = ""){
            _errorCode = errorCode;
            _errorString = errString;
        }

        public int Code => _errorCode;

        public string Message => _errorString;
    }
}
