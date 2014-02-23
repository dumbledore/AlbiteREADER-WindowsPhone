using Albite.Reader.Core.Serialization;

namespace Albite.Reader.Engine.Internal
{
    internal class ClientNotifier : IClientNotifier
    {
        private readonly IEnginePresenter presenter;

        public ClientNotifier(IEnginePresenter presenter)
        {
            this.presenter = presenter;
        }

        public string NotifyClient(string message)
        {
            return presenter.SendMessage(message);
        }
    }
}
