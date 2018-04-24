using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.Utils
{
    public class ThreadUtils
    {
        public static void JoinIgnoreErrors(Thread thread, int waitTime = 1000) {
            try {
                thread.Join(waitTime);

                if (thread.ThreadState != ThreadState.Stopped &&
                    thread.ThreadState != ThreadState.Aborted && 
                    thread.ThreadState != ThreadState.AbortRequested) 
                {
                    thread.Abort();
                }
            } catch {
                ;
            }
        }
    }
}
