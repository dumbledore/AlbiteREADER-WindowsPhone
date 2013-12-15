using System.Windows.Controls;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface INavigationTransitionFactory
    {
        INavigationTransition CreateTransition(ContentControl control, NavigationTransitionMode mode);
    }
}
