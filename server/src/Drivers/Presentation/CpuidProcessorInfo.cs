using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Kernel;
using server.Utils;
using server.Utils.Natives;

namespace server.Drivers.Presentation
{
    public class CpuidProcessorInfo
    {
        public readonly Dictionary<uint, List<CpuidThreadInfo>> Cores;

        public CpuidThreadInfo this[uint core, uint thread] {
            get {
                Cores.TryGetValue(core, out var coreList);
                return coreList?[(int) thread];
            }
        }

        public int GetCoresCount() {
            return Cores.Keys.Count;
        }

        public int GetThreadsCount() {
            return Cores.Sum(x => x.Value.Count);
        }

        public bool IsModelSpecificRegistersSupported {
            get;
            private set;
        }

        public bool IsTimeStampCounterSupported {
            get;
            private set;
        }

        public bool IsInvariantTimeStampCounterSupported {
            get;
            private set;
        }

        public double TimeStampCounterFrequency {
            get;
            private set;
        }

        private readonly double _estimatedTimeStampCounterFrequency;
        private readonly double _estimatedTimeStampCounterFrequencyError;

        public CpuidProcessorInfo(List<CpuidThreadInfo> threadsInfo) {
            Cores = new Dictionary<uint, List<CpuidThreadInfo>>();
            PassThreadsByCore(threadsInfo);

            var core0Info = Cores[0][0];
            IsModelSpecificRegistersSupported = core0Info.DataRegisters.Length > 1 && (core0Info.DataRegisters[1].Edx & 0x20) != 0;
            IsTimeStampCounterSupported = core0Info.DataRegisters.Length > 1 && (core0Info.DataRegisters[1].Edx & 0x10) != 0;
            IsInvariantTimeStampCounterSupported = core0Info.DataRegisters.Length > 1 && (core0Info.DataRegisters[7].Edx & 0x100) != 0;

            if (IsTimeStampCounterSupported) {
                var mask = Native.SetThreadAffinity(1UL << (int)core0Info.ThreadIndex);

                EstimateTimeStampCounterFrequency(
                    out _estimatedTimeStampCounterFrequency,
                    out _estimatedTimeStampCounterFrequencyError);

                Native.SetThreadAffinity(mask);
            } else {
                _estimatedTimeStampCounterFrequencyError = 0;
            }

            TimeStampCounterFrequency = _estimatedTimeStampCounterFrequency;
        }

        private void PassThreadsByCore(List<CpuidThreadInfo> threadsInfo) {
            foreach (var threadInfo in threadsInfo) {
                Cores.TryGetValue(threadInfo.CoreId, out var coreList);
                if (coreList == null) {
                    coreList = new List<CpuidThreadInfo>();
                    Cores.Add(threadInfo.CoreId, coreList);
                }

                coreList.Add(threadInfo);
            }
        }

        public static bool GetCpuLoadTime(out long[] idle, out long[] total) {
            var informations = new SSystemProcessorPerformanceInformation[64];
            var size = Marshal.SizeOf(typeof(SSystemProcessorPerformanceInformation));

            idle = null;
            total = null;

            if (Native.NtQuerySystemInformation(
                    SystemInformationClass.SystemProcessorPerformanceInformation,
                    informations,
                    informations.Length * size,
                    out var returnLength) != 0) {
                return false;
            }

            idle = new long[(int)returnLength / size];
            total = new long[(int)returnLength / size];

            for (var i = 0; i < idle.Length; i++) {
                idle[i] = informations[i].IdleTime;
                total[i] = informations[i].KernelTime + informations[i].UserTime;
            }

            return true;
        }

        private static void EstimateTimeStampCounterFrequency(out double frequency, out double error) {
            // Preload
            EstimateTimeStampCounterFrequency(0, out var f, out var e);
            EstimateTimeStampCounterFrequency(0, out f, out e);

            // Estimate frequency
            error = double.MaxValue;
            frequency = 0;
            for (var i = 0; i < 5; i++) {
                EstimateTimeStampCounterFrequency(0.025, out f, out e);
                if (e < error) {
                    error = e;
                    frequency = f;
                }

                if (error < 1e-4) {
                    break;
                }
            }
        }

        private static void EstimateTimeStampCounterFrequency(double timeWindow, out double frequency, out double error) {
            var ticks = (long)(timeWindow * Stopwatch.Frequency);
            ulong countBegin = 0, countEnd = 0;

            var timeBegin = Stopwatch.GetTimestamp() + (long)Math.Ceiling(0.001 * ticks);
            var timeEnd = timeBegin + ticks;

            while (Stopwatch.GetTimestamp() < timeBegin) {
                ;
            }

            Tsc.Read(ref countBegin);
            var afterBegin = Stopwatch.GetTimestamp();

            while (Stopwatch.GetTimestamp() < timeEnd) {
                ;
            }

            Tsc.Read(ref countEnd);
            var afterEnd = Stopwatch.GetTimestamp();

            double delta = timeEnd - timeBegin;
            frequency = 1e-6 * (((double)(countEnd - countBegin)) * Stopwatch.Frequency) / delta;

            var beginError = (afterBegin - timeBegin) / delta;
            var endError = (afterEnd - timeEnd) / delta;
            error = beginError + endError;
        }
    }
}
