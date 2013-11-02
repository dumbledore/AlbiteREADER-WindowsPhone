using SvetlinAnkov.Albite.Core.Diagnostics;

namespace SvetlinAnkov.Albite.Engine
{
    internal class HostMessenger : AlbiteMessenger.IHostMessenger
    {
        private static readonly string tag = "HostMessenger";

        private readonly AbstractEngine engine;

        public HostMessenger(AbstractEngine engine)
        {
            this.engine = engine;
        }

        public void ClientLog(string message)
        {
            Log.D(tag, "Client: " + message);
        }

        public void ClientLoaded(int page, int pageCount)
        {
            // Don't update Page as it will cause a GoToPageMessage
            // No need to do it anyway

            // Inform the EngineController that it's ready
            engine.Controller.LoadingCompleted();

            // Handle missed orientations
            engine.UpdateDimensions();
        }

        public void ClientLoading(int progress)
        {
            engine.Controller.LoadingProgressed(progress);
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
