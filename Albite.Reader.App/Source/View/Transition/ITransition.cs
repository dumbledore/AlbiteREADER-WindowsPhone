using System;

namespace Albite.Reader.App.View.Transition
{
    public interface ITransition
    {
        void Begin();
        void Stop();
        event EventHandler Completed;
    }
}
