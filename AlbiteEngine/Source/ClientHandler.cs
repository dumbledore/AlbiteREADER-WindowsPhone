using SvetlinAnkov.Albite.Core.Diagnostics;

namespace SvetlinAnkov.Albite.Engine
{
    internal class ClientHandler : EngineMessenger.IClientHandler
    {
        private static readonly string tag = "HostHandler";

        private readonly AlbiteEngine engine;

        public ClientHandler(AlbiteEngine engine)
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
    }
}
