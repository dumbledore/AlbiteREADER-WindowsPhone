using Albite.Reader.Speech.Narration.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Albite.Reader.Speech.Narration
{
    public abstract class Narrator<TExpression> where TExpression : NarrationExpression
    {
        protected NarrationCommand Root { get; private set; }

        private readonly NarrationState<TExpression> state;
        private NarratorThread<TExpression> thread;
        private object myLock = new object();

        protected Narrator(NarrationCommand root, NarrationSettings settings)
        {
            // Set up root command
            Root = root;

            // Now set up default state
            state = new NarrationState<TExpression>(root, settings.BaseLanguage, settings.BaseSpeedRatio);
        }

        public NarrationCommand Command
        {
            get { return state.Command; }
        }

        public TExpression Expression
        {
            get { return state.Expression; }
        }

        public void ReadAsync()
        {
            lock (myLock)
            {
                if (thread != null)
                {
                    throw new InvalidOperationException("Still reading");
                }

                thread = new NarratorThread<TExpression>(state);
                thread.Start();
            }
        }

        public void CancelAsync()
        {
            lock (myLock)
            {
                if (thread == null)
                {
                    return;
                }

                thread.Stop();
                thread = null;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            NarrationCommand command = Root;
            while (command != null)
            {
                b.Append(command);
                b.Append('\n');
                command = command.Next;
            }

            return b.ToString();
        }
    }
}
