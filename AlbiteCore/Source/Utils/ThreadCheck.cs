using System;
using System.Diagnostics;
using System.Threading;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class ThreadCheck
    {
        private readonly int threadId = getThreadId();

        /// <summary>
        /// Checks for thread safety in debug builds. It's not 100%
        /// reliable, but it still can help spotting mistakes
        /// </summary>
        [Conditional("DEBUG")]
        public void Check()
        {
            int newThreadId = getThreadId();

            if (threadId != newThreadId)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Thread #{0} is different from #{1}",
                        newThreadId, threadId
                    )
                );
            }
        }

        private static int getThreadId() {
            return Thread.CurrentThread.ManagedThreadId;
        }
    }
}
