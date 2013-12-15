using System;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface ITransition
    {
        void Begin();
        void Stop();
        event EventHandler Completed;
    }
}
