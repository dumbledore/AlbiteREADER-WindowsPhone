using System.Threading;
using Albite.Reader.Speech.Narration.Commands;

namespace Albite.Reader.Speech.Narration
{
    internal abstract class NarrationEngine<TExpression> : INarrationEngine where TExpression : NarrationExpression
    {
        protected readonly CancellationToken Token;
        protected readonly NarrationState<TExpression> NarrationState;

        protected NarrationEngine(NarrationState<TExpression> state, CancellationToken token)
        {
            this.NarrationState = state;
            this.Token = token;
        }

        public INarrationState State
        {
            get { return NarrationState; }
        }

        public abstract void Read(string text);

        public void Pause(int duration)
        {
            Token.WaitHandle.WaitOne(duration);
        }
    }
}
