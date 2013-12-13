using Microsoft.Phone.Controls;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public interface ITransitionFactory
    {
        ITransition CreateTransition(PhoneApplicationFrame frame, ITransitionMode mode);
    }
}
