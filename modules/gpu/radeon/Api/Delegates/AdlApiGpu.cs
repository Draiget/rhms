using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Helpers;

// ReSharper disable InconsistentNaming
namespace gpu_radeon.Api.Delegates
{
    public static class AdlApiGpu
    {
        public delegate IntPtr ADL_Main_Memory_AllocDelegate(int size);

        [LinkedDelegate("ADL_Main_Control_Create")]
        public delegate int ADL_Main_Control_CreateDelegate(ADL_Main_Memory_AllocDelegate callback, int enumConnectedAdapters);

        [LinkedDelegate("ADL_Main_Control_Destroy")]
        public delegate int ADL_Main_Control_DestroyDelegate();

        [LinkedDelegate("ADL_Adapter_AdapterInfo_Get")]
        public delegate int ADL_Adapter_AdapterInfo_GetDelegate(IntPtr info, int size);
    }
}
