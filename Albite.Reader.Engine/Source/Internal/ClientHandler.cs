using Albite.Reader.Core.Diagnostics;

namespace Albite.Reader.Engine.Internal
{
    internal class ClientHandler : EngineMessenger.IClientHandler
    {
        private static readonly string tag = "HostHandler";

        private readonly Engine engine;

        public ClientHandler(Engine engine)
        {
            this.engine = engine;
        }

        public void ClientLog(string message)
        {
            Log.D(tag, "Client: " + message);
        }

        public void ClientLoaded(int page, int pageCount)
        {
            engine.OnClientLoaded(page, pageCount);
        }

        public void GoToPreviousChapter()
        {
            engine.Navigator.GoToPreviousChapter();
        }

        public void GoToNextChapter()
        {
            engine.Navigator.GoToNextChapter();
        }

        public void ClientNavigationRequest(string url, string title)
        {
            engine.OnNavigationRequested(url, title);
        }
    }
}
