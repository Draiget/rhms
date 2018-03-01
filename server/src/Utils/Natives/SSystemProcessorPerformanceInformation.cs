using System.Runtime.InteropServices;

namespace server.Utils.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SSystemProcessorPerformanceInformation
    {
        public long IdleTime;
        public long KernelTime;
        public long UserTime;
        public long Reserved0;
        public long Reserved1;
        public ulong Reserved2;
    }
}
