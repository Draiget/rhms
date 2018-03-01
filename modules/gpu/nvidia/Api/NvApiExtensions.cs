using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace gpu_nvidia.Api
{
    public static class NvApiExtensions
    {
        public static TResult BitwiseConvert<TResult, T>(T source) where TResult : struct, IConvertible
            where T : struct, IConvertible {
            if (typeof(T) == typeof(TResult))
                return (TResult)(object)source;
            var sourceSize = Marshal.SizeOf(typeof(T));
            var destinationSize = Marshal.SizeOf(typeof(TResult));
            var minSize = Math.Min(sourceSize, destinationSize);
            var sourcePointer = Marshal.AllocHGlobal(sourceSize);
            Marshal.StructureToPtr(source, sourcePointer, false);
            var bytes = new byte[destinationSize];
            if (BitConverter.IsLittleEndian)
                Marshal.Copy(sourcePointer, bytes, 0, minSize);
            else
                Marshal.Copy(sourcePointer + (sourceSize - minSize), bytes, destinationSize - minSize, minSize);
            Marshal.FreeHGlobal(sourcePointer);
            var destinationPointer = Marshal.AllocHGlobal(destinationSize);
            Marshal.Copy(bytes, 0, destinationPointer, destinationSize);
            var destination = (TResult)Marshal.PtrToStructure(destinationPointer, typeof(TResult));
            Marshal.FreeHGlobal(destinationPointer);
            return destination;
        }

        public static bool GetBit<T>(this T integer, int index) where T : struct, IConvertible {
            var bigInteger = BitwiseConvert<ulong, T>(integer);
            return (bigInteger & (1ul << index)) != 0;
        }

        public static T[] Repeat<T>(this T structure, int count) {
            return Enumerable.Range(0, count).Select(i => structure).ToArray();
        }

        public static T SetBit<T>(this T integer, int index, bool value) where T : struct, IConvertible {
            var bigInteger = BitwiseConvert<ulong, T>(integer);
            var mask = 1ul << index;
            var newInteger = value ? bigInteger | mask : bigInteger & ~mask;
            return BitwiseConvert<T, ulong>(newInteger);
        }
    }
}
