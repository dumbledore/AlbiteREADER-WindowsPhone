using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
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

        public bool IsLoading { get; private set; }

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

            if (IsLoading)
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

        /// <summary>
        /// Once we have a valid client, i.e. after the first load
        /// has completed, we can request the location
        /// </summary>
        private bool canGetDomLocation = false;

        /// <summary>
        /// This ensures that the client is in the right state,
        /// i.e. loaded and responsive
        /// </summary>
        protected void EnsureValidState()
        {
            if (IsLoading)
            {
                throw new InvalidOperationException("Client not loaded yet");
            }
        }

        internal void OnClientLoaded(int page, int pageCount)
        {
            navigator.PageCount = pageCount;

            // Inform the EngineController that it's ready
            EngineController.LoadingCompleted();
            IsLoading = false;

            // We now have a working client
            canGetDomLocation = true;

            // Handle missed orientations
            UpdateDimensions();
        }

        /// <summary>
        /// Called before switching chapters
        /// </summary>
        internal void TryPersist()
        {
            if (canGetDomLocation)
            {
                // Cache the location
                BookPresenter.BookLocation = Navigator.BookLocation;
            }
        }

        internal void SetChapter(string fileUrl, InitialLocation initialLocation)
        {
            // Set up main.xhtml
            TemplateController.UpdateChapter(
                initialLocation,
                Navigator.IsFirstChapter, Navigator.IsLastChapter,
                Path.Combine("/" + BookPresenter.RelativeContentPath, fileUrl));

            ReloadBrowser();
        }

        /// <summary>
        /// Reload the current chapter and navigate to the current DomLocation
        /// </summary>
        private void Reload()
        {
            // We *must* use the template mechanism because of the
            // rendering synchronization of the page jump at start

            BookLocation bookLocation = Navigator.BookLocation;

            // Update the location in the BookPresenter
            BookPresenter.BookLocation = bookLocation;

            InitialLocation initialLocation =
                InitialLocation.GetDomLocation(bookLocation.DomLocation);

            TemplateController.UpdateInitialLocation(initialLocation);

            // Now reload the web browser
            ReloadBrowser();
        }

        private void ReloadBrowser()
        {
            IsLoading = true;
            EngineController.LoadingStarted();
            EngineController.ReloadBrowser();
        }
    }
}
