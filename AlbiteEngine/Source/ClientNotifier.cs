using SvetlinAnkov.Albite.Core.Serialization;

namespace SvetlinAnkov.Albite.Engine
{
    internal class ClientNotifier : IClientNotifier
    {
        private readonly IEngineController controller;

        public ClientNotifier(IEngineController controller)
        {
            this.controller = controller;
        }

        public string NotifyClient(string message)
        {
            return controller.SendMessage(message);
        }
    }
}
