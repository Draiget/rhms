using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gpu_radeon.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AdlAdapterInfo
    {
        public int Size;
        public int AdapterIndex;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlApi.AdlMaxPath)]
        public string UDID;

        public int BusNumber;
        public int DeviceNumber;
        public int FunctionNumber;
        public int VendorID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlApi.AdlMaxPath)]
        public string AdapterName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlApi.AdlMaxPath)]
        public string DisplayName;

        public int Present;
        public int Exist;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlApi.AdlMaxPath)]
        public string DriverPath;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlApi.AdlMaxPath)]
        public string DriverPathExt;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = AdlApi.AdlMaxPath)]
        public string PNPString;

        public int OSDisplayIndex;
    }
}
