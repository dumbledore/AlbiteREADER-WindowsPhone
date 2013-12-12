using System.Windows;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface ITransitionFactory
    {
        ITransition CreateTransition(UIElement root, ITransitionMode mode);
    }
}
