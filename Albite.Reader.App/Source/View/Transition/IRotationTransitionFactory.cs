using System.Windows;

namespace Albite.Reader.App.View.Transition
{
    public interface IRotationTransitionFactory
    {
        IRotationTransition CreateTransition(UIElement element, RotationTransitionMode mode);
    }
}
