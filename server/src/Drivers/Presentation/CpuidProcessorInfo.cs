using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        public CpuidProcessorInfo(List<CpuidThreadInfo> threadsInfo) {
            Cores = new Dictionary<uint, List<CpuidThreadInfo>>();
            PassThreadsByCore(threadsInfo);
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
    }
}
