namespace Albite.Reader.App.View.Transition
{
    public interface INavigationTransition : ITransition
    {
        NavigationTransitionMode Mode { get; }
    }
}