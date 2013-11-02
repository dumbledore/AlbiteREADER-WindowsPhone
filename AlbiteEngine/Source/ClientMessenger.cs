namespace SvetlinAnkov.Albite.Engine
{
    internal class ClientMessenger : AlbiteMessenger.IClientMessenger
    {
        private readonly IEngineController controller;

        public ClientMessenger(IEngineController controller)
        {
            this.controller = controller;
        }

        public string NotifyClient(string message)
        {
            return controller.SendMessage(message);
        }
    }
}
