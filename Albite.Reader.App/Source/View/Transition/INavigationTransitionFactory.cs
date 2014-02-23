using System.Windows.Controls;

namespace Albite.Reader.App.View.Transition
{
    public interface INavigationTransitionFactory
    {
        INavigationTransition CreateTransition(ContentControl control, NavigationTransitionMode mode);
    }
}
