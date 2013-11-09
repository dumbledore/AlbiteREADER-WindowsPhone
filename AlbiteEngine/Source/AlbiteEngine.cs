using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
using System;
using System.IO;

namespace SvetlinAnkov.Albite.Engine
{
    public class AlbiteEngine : IEngine
    {
        private static readonly string tag = "AlbiteEngine";

        public BookPresenter BookPresenter { get; private set; }
        public Settings Settings { get; private set; }

        public Uri Uri { get; private set; }

        private EngineNavigator navigator;
        public IEngineNavigator Navigator
        {
            get
            {
                EnsureValidState();
                return navigator;
            }
        }

        internal IEngineController EngineController { get; private set; }
        internal EngineMessenger Messenger { get; private set; }

        private EngineTemplateController TemplateController;

        public AlbiteEngine(
            IEngineController engineController, BookPresenter bookPresenter, Settings settings)
        {
            EngineController = engineController;
            BookPresenter = bookPresenter;
            Settings = settings;

            // Set up the base path
            EngineController.BasePath = BookPresenter.Path;

            // Uri of main.xhtml
            Uri = new Uri(
                Path.Combine(BookPresenter.RelativeEnginePath, Paths.MainPage),
                UriKind.Relative);

            // Prepare the template controller
            TemplateController = new EngineTemplateController(
                Settings, BookPresenter.EnginePath,
                EngineController.Width, EngineController.Height);

            // Create the messenger for the engine
            Messenger = new EngineMessenger(
                new ClientHandler(this),
                new ClientNotifier(EngineController)
            );

            navigator = new EngineNavigator(this);
        }

        public void UpdateLayout()
        {
            // This can't happen while loading
            EnsureValidState();

            // Update the templates
            TemplateController.UpdateSettings();

            // Reload to the current DomLocation
            Reload();
        }

        public void UpdateDimensions()
        {
            // It is perfectly legal to be called before
            // the client has fully loaded, because
            // the layout could be updated at any time

            // Get current dimensions
            int width = TemplateController.Width;
            int height = TemplateController.Height;

            // New dimensions
            int newWidth = EngineController.Width;
            int newHeight = EngineController.Height;

            if (EngineController.IsLoading)
            {
                Log.D(tag, "Can't update the dimensions while loading");
                return;
            }

            if (newWidth == width
                && newHeight == height)
            {
                Log.D(tag, "The dimensions haven't changed, no need to update.");
                return;
            }

            // Update the templates
            TemplateController.UpdateDimensions(newWidth, newHeight);

            // Reload to the current DomLocation
            Reload();
        }

        public void ReceiveMessage(string message)
        {
            Messenger.NotifyHost(message);
        }

        protected void EnsureValidState()
        {
            if (EngineController.IsLoading)
            {
                throw new InvalidOperationException("Client not loaded yet");
            }
        }

        internal void OnClientLoaded(int page, int pageCount)
        {
            navigator.PageCount = pageCount;

            // Inform the EngineController that it's ready
            EngineController.LoadingCompleted();

            // Handle missed orientations
            UpdateDimensions();
        }

        internal void SetChapter(string fileUrl, InitialLocation initialLocation)
        {
            // Set up main.xhtml
            TemplateController.UpdateChapter(
                initialLocation,
                Navigator.IsFirstChapter, Navigator.IsLastChapter,
                Path.Combine("/" + BookPresenter.RelativeContentPath, fileUrl));

            EngineController.ReloadBrowser();
        }

        /// <summary>
        /// Reload the current chapter and navigate to the current DomLocation
        /// </summary>
        private void Reload()
        {
            // We *must* use the template mechanism because of the
            // rendering synchronization of the page jump at start

            InitialLocation initialLocation =
                InitialLocation.GetDomLocation(navigator.DomLocation);
            TemplateController.UpdateInitialLocation(initialLocation);

            // Now reload the web browser
            EngineController.ReloadBrowser();
        }
    }
}
