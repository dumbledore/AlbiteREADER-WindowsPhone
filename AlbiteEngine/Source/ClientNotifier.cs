using SvetlinAnkov.Albite.Core.Serialization;

namespace SvetlinAnkov.Albite.Engine
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
