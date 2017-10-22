using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.Utils.Logging
{
    public struct SLogMessageHolder
    {
        public readonly string Message;
        public readonly long Time;
        public readonly ELogLevel Level;
        public readonly Thread ContextThread;

        public SLogMessageHolder(string msg, ELogLevel level, Thread contextThread) {
            ContextThread = contextThread;
            Level = level;
            Message = msg;
            Time = DateTime.Now.Ticks;
        }
    }
}
