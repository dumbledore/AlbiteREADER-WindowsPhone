using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Threading;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public abstract class BrowserEngine
    {
        private static readonly string tag = "BrowserEngine";

        private AlbiteMessenger messenger;

        #region Constructors
        public BrowserEngine(IEngineController controller, Settings settings)
        {
            this.Controller = controller;
            this.settings = settings;
            messenger = new AlbiteMessenger(
                new ClientMessenger(controller),
                new HostMessenger(this)
            );

            prepare();
        }
        #endregion

        #region Public API

        public Chapter Chapter { get; private set; }

        public void SetChapter(Chapter chapter, int page)
        {
            mainPageTemplate.InitialLocation = page.ToString();
            SetChapter(chapter);
        }

        public void SetChapterFirstPage(Chapter chapter)
        {
            mainPageTemplate.InitialLocation = "'first'";
            SetChapter(chapter);
        }

        public void SetChapterLastPage(Chapter chapter)
        {
            mainPageTemplate.InitialLocation = "'last'";
            SetChapter(chapter);
        }

        public void SetChapterDomLocation(Chapter chapter, string location)
        {
            mainPageTemplate.InitialLocation = string.Format("'{0}'", location);

            SetChapter(chapter);
        }

        private void SetChapter(Chapter chapter)
        {
            Controller.LoadingStarted();

            Chapter = chapter;

            // Set up the main.xhtml
            mainPageTemplate.ChatperFile = Path.Combine("/" + Controller.Presenter.RelativeContentPath, chapter.Url);
            mainPageTemplate.SaveToStorage();

            // Now navigate the web browser
            navigateBrowser();
        }

        public string DomLocation
        {
            get
            {
                //TODO: Write JScript code that will tell the current reading location
                //      using ScriptNotify() and window.external.notify().
                return "";
            }

            set { goToDomLocation(value); }
        }

        /// <summary>
        /// Gets / sets the current page
        /// </summary>
        public int Page
        {
            get { return 0 /* TODO: Ask JS Client */; }
            set { goToPage(value); }
        }

        public int PageCount { get; private set; }

        public void GoToFirstPage()
        {
            goToPage(FirstPageNumber);
        }

        public void GoToLastPage()
        {
            goToPage(LastPageNumber);
        }

        //Note: there are always AT LEAST 3 pages
        public int FirstPageNumber { get { return 1; } }
        public int LastPageNumber { get { return PageCount - 2; } }
        #endregion

        #region Location implementation

        private void goToDomLocation(string location)
        {
            if (Controller.IsLoading)
            {
                return;
            }

            // TODO
        }

        private void goToPage(int pageNumber)
        {
            // TODO
        }
        #endregion

        #region Browser Navigation

        private void reloadBrowser()
        {
            if (Controller.SourceUri != mainUri)
            {
                return;
            }

            // Navigate to the same chapter and the
            // current DomLocation
            SetChapterDomLocation(Chapter, DomLocation);
        }

        private void navigateBrowser()
        {
            Controller.SourceUri = mainUri;
        }

        /// <summary>
        /// Navigate to a in-book or out-book uri
        /// </summary>
        /// <param name="uri">uri</param>
        /// <returns>True, if navigation is handled</returns>
        public bool NavigateTo(Uri uri)
        {
            if (uri == mainUri)
            {
                // Allow the browser to load
                return false;
            }

            // TODO: Handle in-book & out-book navigation
            return true;
        }
        #endregion

        #region Templates

        private Uri mainUri;

        // The settings are read-only, because their values will be updated
        // through data binding.
        private readonly Settings settings;
        public Settings Settings
        {
            get { return settings; }
        }

        private MainPageTemplate mainPageTemplate;
        private BaseStylesTemplate baseStylesTemplate;
        private ContentStylesTemplate contentStylesTemplate;

        private void prepare()
        {
            Controller.BasePath = Controller.Presenter.Path;

            mainUri = new Uri(Path.Combine(Controller.Presenter.RelativeEnginePath, Paths.MainPage), UriKind.Relative);

            string enginePath = Controller.Presenter.EnginePath;

            // Copy the JSEngine to the Isolated Storage
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(
                EngineTemplate.GetOutputPath(Paths.JSEngine, enginePath)))
            {
                using (AlbiteStorage res = EngineTemplate.GetStorage(Paths.JSEngine))
                {
                    res.CopyTo(iso);
                }
            }

            // Load the templates
            mainPageTemplate = new MainPageTemplate(enginePath);
#if DEBUG
            mainPageTemplate.DebugEnabled = true;
#else
            mainPageTemplate.DebugEnabled = false;
#endif
            baseStylesTemplate = new BaseStylesTemplate(enginePath);
            contentStylesTemplate = new ContentStylesTemplate(enginePath);

            // Set up the templates
            updateLayout();

            // Set up the dimensions
            updateDimensions(Controller.ViewportWidth, Controller.ViewportHeight);

            // Set up the theme
            updateTheme();
        }

        private void updateTheme()
        {
            baseStylesTemplate.BackgroundColor = settings.Theme.BackgroundColor;
            baseStylesTemplate.TextColor = settings.Theme.FontColor;
            baseStylesTemplate.SaveToStorage();

            contentStylesTemplate.BackgroundColor = settings.Theme.BackgroundColor;
            contentStylesTemplate.TextColor = settings.Theme.FontColor;
            contentStylesTemplate.AccentColor = settings.Theme.AccentColor;
            contentStylesTemplate.SaveToStorage();
        }

        private int actualViewportWidth = 0;
        private int actualViewportHeight = 0;

        /// <summary>
        /// Called whenever the viewport is resized and/or the margins
        /// have been changed.
        /// </summary>
        public void UpdateDimensions()
        {
            int viewportWidth = Controller.ViewportWidth;
            int viewportHeight = Controller.ViewportHeight;

            if (Controller.IsLoading)
            {
                Log.D(tag, "Can't update the dimensions while loading");
                return;
            }

            if (viewportWidth == actualViewportWidth
                && viewportHeight == actualViewportHeight)
            {
                Log.D(tag, "The dimensions haven't changed, no need to update.");
                return;
            }

            updateDimensions(viewportWidth, viewportHeight);
            reloadBrowser();
        }

        private void updateDimensions(int viewportWidth, int viewportHeight)
        {
            Log.D(tag, string.Format("UpdateDimensions: {0}x{1}", viewportWidth, viewportHeight));

            mainPageTemplate.Width = viewportWidth;
            mainPageTemplate.Height = viewportHeight;
            mainPageTemplate.SaveToStorage();

            baseStylesTemplate.Width = viewportWidth;
            baseStylesTemplate.Height = viewportHeight;
            baseStylesTemplate.SaveToStorage();

            int viewportReference = Math.Max(viewportWidth, viewportWidth);
            int marginLeft = (int) (settings.MarginLeft * viewportReference);
            int marginRight = (int) (settings.MarginRight * viewportReference);
            int marginTop = (int) (settings.MarginTop * viewportReference);
            int marginBottom = (int) (settings.MarginBottom * viewportReference);

            contentStylesTemplate.MarginTop = marginTop;
            contentStylesTemplate.MarginBottom = marginBottom;
            contentStylesTemplate.MarginLeft = marginLeft;
            contentStylesTemplate.MarginRight = marginRight;

            contentStylesTemplate.Width = viewportWidth;
            contentStylesTemplate.Height = viewportHeight;

            contentStylesTemplate.SaveToStorage();

            actualViewportWidth = viewportWidth;
            actualViewportHeight = viewportHeight;
        }

        private void updateLayout()
        {
            contentStylesTemplate.LineHeight = settings.LineHeight;
            contentStylesTemplate.FontSize = settings.FontSize;
            contentStylesTemplate.FontFamily = settings.FontFamily;
            contentStylesTemplate.TextAlign = settings.TextAlign;

            contentStylesTemplate.SaveToStorage();
        }
        #endregion

        #region Handling Messages from the JS Client

        public void ReceiveMessage(string message)
        {
            messenger.NotifyHost(message);
        }

        private class HostMessenger: AlbiteMessenger.IHostMessenger
        {
            private readonly BrowserEngine engine;

            public HostMessenger(BrowserEngine engine)
            {
                this.engine = engine;
            }

            public void ClientLoaded(int page, int pageCount)
            {
                // Inform the EngineController that it's ready
                engine.Controller.LoadingCompleted();

                // Now get the page count
                engine.PageCount = pageCount;

                // Handle missed orientations
                engine.UpdateDimensions();
            }
        }

        private class ClientMessenger : AlbiteMessenger.IClientMessenger
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
        #endregion

        #region IEngineController

        protected readonly IEngineController Controller;

        public interface IEngineController
        {
            int ViewportWidth { get; }
            int ViewportHeight { get; }

            string BasePath { get; set; }
            Uri SourceUri { get; set; }

            string SendMessage(string message);

            Book.Presenter Presenter { get; }

            bool IsLoading { get; }
            void LoadingStarted();
            void LoadingCompleted();

            void OnError(string message);
        }
        #endregion
    }
}
