using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace gpu_nvidia.Api.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NvBoardInfo : IEquatable<NvBoardInfo>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] _SerialNumber;

        public byte[] SerialNumber => _SerialNumber;

        public bool Equals(NvBoardInfo other) {
            return _SerialNumber.SequenceEqual(other._SerialNumber);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is NvBoardInfo && Equals((NvBoardInfo)obj);
        }

        public override int GetHashCode() {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _SerialNumber?.GetHashCode() ?? 0;
        }

        public override string ToString() {
            return SerialNumber == null ? "Unknown" : "Serial " + BitConverter.ToString(SerialNumber);
        }

        public static bool operator ==(NvBoardInfo left, NvBoardInfo right) {
            return left.Equals(right);
        }

        public static bool operator !=(NvBoardInfo left, NvBoardInfo right) {
            return !left.Equals(right);
        }
    }
}
