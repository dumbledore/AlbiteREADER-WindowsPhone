using System;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface ITransition
    {
        void Begin();
        void Stop();
        ITransitionMode Mode { get; }
        event EventHandler Completed;
    }
}
