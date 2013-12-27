using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Engine.Internal;
using SvetlinAnkov.Albite.Engine.Layout;
using System;
using System.IO;

namespace SvetlinAnkov.Albite.Engine
{
    public class AlbiteEngine : IEngine
    {
        private static readonly string tag = "AlbiteEngine";

        public BookPresenter BookPresenter { get; private set; }
        public LayoutSettings Settings { get; private set; }

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

        internal IEnginePresenter EnginePresenter { get; private set; }
        internal EngineMessenger Messenger { get; private set; }

        private EngineTemplateController TemplateController;

        public AlbiteEngine(
            IEnginePresenter enginePresenter, BookPresenter bookPresenter, LayoutSettings settings)
        {
            EnginePresenter = enginePresenter;
            BookPresenter = bookPresenter;
            Settings = settings;

            // Set up the base path
            EnginePresenter.BasePath = BookPresenter.Path;

            // Uri of main.xhtml
            Uri = new Uri(
                Path.Combine(BookPresenter.RelativeEnginePath, Paths.MainPage),
                UriKind.Relative);

            // Prepare the template controller
            TemplateController = new EngineTemplateController(
                Settings, BookPresenter.EnginePath,
                EnginePresenter.Width, EnginePresenter.Height,
                EnginePresenter.ApplicationBarHeight);

            // Create the messenger for the engine
            Messenger = new EngineMessenger(
                new ClientHandler(this),
                new ClientNotifier(EnginePresenter)
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

        public bool UpdateDimensions()
        {
            // It is perfectly legal to be called before
            // the client has fully loaded, because
            // the layout could be updated at any time

            // Get current dimensions
            int width = TemplateController.Width;
            int height = TemplateController.Height;

            // New dimensions
            int newWidth = EnginePresenter.Width;
            int newHeight = EnginePresenter.Height;

            if (IsLoading)
            {
                Log.D(tag, "Can't update the dimensions while loading");
                return false;
            }

            if (newWidth == width
                && newHeight == height)
            {
                Log.D(tag, "The dimensions haven't changed, no need to update.");
                return false;
            }

            // Update the templates
            TemplateController.UpdateDimensions(newWidth, newHeight, EnginePresenter.ApplicationBarHeight);

            // Reload to the current DomLocation
            Reload();

            return true;
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

            // Not loading anymore
            IsLoading = false;

            // We now have a working client
            canGetDomLocation = true;

            // Handle missed orientations
            bool needToReload = UpdateDimensions();

            if (!needToReload)
            {
                // No need to reload. Without further ado
                // inform the EnginePresenter that we're ready
                EnginePresenter.LoadingCompleted();
            }
        }

        private static readonly string absoluteContentPath
            = Path.Combine("/", BookPresenter.RelativeContentPath) + "/";

        internal void OnNavigationRequested(string url)
        {
            bool handled = false;

            Uri uri;
            bool isAbsolute = Uri.TryCreate(url, UriKind.Absolute, out uri);
            if (isAbsolute)
            {
                if (uri.IsFile
                    || uri.Scheme.StartsWith("x-wmapp", StringComparison.OrdinalIgnoreCase))
                {
                    // Get the absolute path
                    string path = uri.AbsolutePath;

                    // Is it a valid path?
                    if (path.StartsWith(absoluteContentPath)
                        && path.Length > absoluteContentPath.Length)
                    {
                        // Remove the initial part of the path, e.g. "/content"
                        path = path.Substring(absoluteContentPath.Length);

                        // Try looking for the chapter with this path
                        Chapter chapter = BookPresenter.Spine[path];

                        if (chapter != null)
                        {
                            string fragment = uri.Fragment;
                            ChapterLocation chapterLocation;
                            if (fragment.Length > 1)
                            {
                                // Go to fragment. It starts with a #,
                                // so remove the first character
                                chapterLocation = new ElementLocation(fragment.Substring(1));
                            }
                            else
                            {
                                // Go to beginning of chapter
                                chapterLocation = new FirstPageLocation();
                            }

                            // Get the BookLocation
                            BookLocation bookLocation = chapter.CreateLocation(chapterLocation);

                            // Inform the client we're indeed navigating away
                            EnginePresenter.Navigating(Navigator.BookLocation);

                            // Go there
                            Navigator.BookLocation = bookLocation;

                            // Handled, indeed
                            handled = true;
                        }
                    }
                }
                else if (EnginePresenter.NavigationRequested(uri))
                {
                    // Handled by the UI
                    handled = true;
                }
            }

            if (!handled)
            {
                Log.W(tag, "The Uri wasn't handled: " + uri);
            }
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

        internal void SetChapter(string fileUrl, ChapterLocation initialLocation)
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

            TemplateController.UpdateInitialLocation(bookLocation.Location);

            // Now reload the web browser
            ReloadBrowser();
        }

        private void ReloadBrowser()
        {
            IsLoading = true;
            EnginePresenter.LoadingStarted();
            EnginePresenter.ReloadBrowser();
        }
    }
}
