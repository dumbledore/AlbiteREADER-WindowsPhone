namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface INavigationTransition : ITransition
    {
        NavigationTransitionMode Mode { get; }
    }
}