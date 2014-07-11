using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Speech.Narration.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Narration
{
    internal class NarratorThread<TExpression> where TExpression : NarrationExpression
    {
        private static readonly string Tag = "NarratorThread";

        private volatile bool started = false;

        private readonly NarrationState<TExpression> state;
        private readonly NarrationEngine<TExpression> engine;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public NarratorThread(NarrationState<TExpression> state)
        {
            this.state = state;
            this.engine = new NarrationEngineSynthesizer<TExpression>(state, cts.Token);
        }

        public void Start()
        {
            if (started)
            {
                throw new InvalidOperationException("Already running");
            }

            started = true;

            // Start the thread
            ThreadPool.QueueUserWorkItem(threadLoop);
        }

        public void Stop()
        {
            Log.D(Tag, "Stopping thread");
            cts.Cancel();
        }

        private void threadLoop(object s)
        {
            NarrationCommand command = null;

            Log.D(Tag, "Starting thread");

            while (!cts.IsCancellationRequested)
            {
                command = state.Next();

                if (command == null)
                {
                    // No more commands
                    Log.D(Tag, "No more commands");
                    break;
                }

                // Execute the command
                Log.D(Tag, "Executing: " + command);
                command.Execute(engine);
            }

            if (!cts.IsCancellationRequested)
            {
                Log.D(Tag, "Finished reading");
            }

            Log.D(Tag, "Thread finished");
        }
    }
}
