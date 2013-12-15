using System.Windows;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface IRotationTransitionFactory
    {
        IRotationTransition CreateTransition(UIElement element, RotationTransitionMode mode);
    }
}
