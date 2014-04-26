using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.Core.Threading
{
    /// <summary>
    /// Used to handle both the Task and
    /// its CancellationTokenSource in one class
    /// </summary>
    public class CancellableTask
    {
        public Task Task { get; private set; }

        private CancellationTokenSource cts;

        public CancellableTask(Task task, CancellationTokenSource cts)
        {
            this.Task = task;
            this.cts = cts;
        }

        public void Cancel()
        {
            if (Task.IsCanceled || Task.IsCompleted || Task.IsFaulted)
            {
                cts = null;
                return;
            }

            // Cancel it
            cts.Cancel();

            // Do not wait for it finish!
            // In out case the task is running on the UI thread,
            // but we are waiting for it on the UI thread,
            // which would obviously cause a dead-lock.

            // cts is not needed anymore
            cts = null;
        }
    }
}
