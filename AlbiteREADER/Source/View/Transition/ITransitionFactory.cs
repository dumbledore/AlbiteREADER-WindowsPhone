using System.Windows.Controls;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface ITransitionFactory
    {
        ITransition CreateTransition(ContentControl control, ITransitionMode mode);
    }
}
