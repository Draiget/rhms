using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.Utils.Logging
{
    public struct LogMessageHolder
    {
        public readonly string Message;
        public readonly long Time;
        public readonly LogLevel Level;
        public readonly Thread ContextThread;

        public LogMessageHolder(string msg, LogLevel level, Thread contextThread) {
            ContextThread = contextThread;
            Level = level;
            Message = msg;
            Time = DateTime.Now.Ticks;
        }
    }
}
