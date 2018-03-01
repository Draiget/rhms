using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpu_radeon.Api.Helpers;
using gpu_radeon.Api.Structures;

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

        [LinkedDelegate("ADL_Adapter_NumberOfAdapters_Get")]
        public delegate int ADL_Adapter_NumberOfAdapters_GetDelegate(ref int numAdapters);

        [LinkedDelegate("ADL_Adapter_ID_Get")]
        public delegate int ADL_Adapter_ID_GetDelegate(int adapterIndex, out int adapterID);

        [LinkedDelegate("ADL_Display_AdapterID_Get")]
        public delegate int ADL_Display_AdapterID_GetDelegate(int adapterIndex, out int adapterID);

        [LinkedDelegate("ADL_Adapter_Active_Get")]
        public delegate int ADL_Adapter_Active_GetDelegate(int adapterIndex, out int status);

        [LinkedDelegate("ADL_Overdrive5_CurrentActivity_Get")]
        public delegate int ADL_Overdrive5_CurrentActivity_GetDelegate(int iAdapterIndex, ref AdlpmActivity activity);

        [LinkedDelegate("ADL_Overdrive5_Temperature_Get")]
        public delegate int ADL_Overdrive5_Temperature_GetDelegate(int adapterIndex, int thermalControllerIndex, ref AdlTemperature temperature);

        [LinkedDelegate("ADL_Overdrive5_FanSpeed_Get")]
        public delegate int ADL_Overdrive5_FanSpeed_GetDelegate(int adapterIndex, int thermalControllerIndex, ref AdlFanSpeedValue fanSpeedValue);

        [LinkedDelegate("ADL_Overdrive5_FanSpeedInfo_Get")]
        public delegate int ADL_Overdrive5_FanSpeedInfo_GetDelegate(int adapterIndex, int thermalControllerIndex, ref AdlFanSpeedInfo fanSpeedInfo);

        [LinkedDelegate("ADL_Overdrive5_FanSpeedToDefault_Set")]
        public delegate int ADL_Overdrive5_FanSpeedToDefault_SetDelegate(int adapterIndex, int thermalControllerIndex);

        [LinkedDelegate("ADL_Overdrive5_FanSpeed_Set")]
        public delegate int ADL_Overdrive5_FanSpeed_SetDelegate(int adapterIndex, int thermalControllerIndex, ref AdlFanSpeedValue fanSpeedValue);
    }
}
