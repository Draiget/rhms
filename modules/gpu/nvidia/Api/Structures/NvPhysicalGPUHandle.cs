using System;
using System.Runtime.InteropServices;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NvPhysicalGpuHandle : IEquatable<NvPhysicalGpuHandle>
    {
        public const int PhysicalGPUs = 32;
        public const int MaxPhysicalGPUs = 64;

        public IntPtr MemoryAddress;

        public bool Equals(NvPhysicalGpuHandle other){
            return MemoryAddress.Equals(other.MemoryAddress);
        }

        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)) return false;
            return obj is NvPhysicalGpuHandle && Equals((NvPhysicalGpuHandle) obj);
        }

        public override int GetHashCode(){
            return MemoryAddress.GetHashCode();
        }
    }
}
