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

        #region Constructors
        public BrowserEngine(IEngineController controller, Settings settings)
        {
            this.Controller = controller;
            this.settings = settings;

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
            mainPageTemplate.InitialLocation = "\"first\"";
            SetChapter(chapter);
        }

        public void SetChapterLastPage(Chapter chapter)
        {
            mainPageTemplate.InitialLocation = "\"last\"";
            SetChapter(chapter);
        }

        public void SetChapterDomLocation(Chapter chapter, DomLocation location)
        {
            mainPageTemplate.InitialLocation
                = string.Format("{{elementIndex: {0}, textOffset:{1} }}",
                location.ElementIndex, location.TextOffset);

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

        public DomLocation DomLocation
        {
            get
            {
                //TODO: Write JScript code that will tell the current reading location
                //      using ScriptNotify() and window.external.notify().
                return new DomLocation(0, 0);
            }

            set { goToDomLocation(value); }
        }

        private int currentPage;

        /// <summary>
        /// Gets / sets the current page
        /// </summary>
        public int Page
        {
            get { return currentPage; }
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

        public bool IsFirstPage { get { return currentPage <= FirstPageNumber; } }
        public bool IsLastPage { get { return currentPage >= LastPageNumber; } }
        #endregion

        #region Location implementation

        private int validatePageNumber(int pageNumber)
        {
            if (pageNumber <= FirstPageNumber)
            {
                pageNumber = FirstPageNumber;
            }

            if (pageNumber >= LastPageNumber)
            {
                pageNumber = LastPageNumber;
            }

            return pageNumber;
        }

        private void goToDomLocation(DomLocation location)
        {
            if (Controller.IsLoading)
            {
                return;
            }

            // setup the arguments
            string[] args = {
                location.ElementIndex.ToString(), location.TextOffset.ToString()
            };

            // get the page for this dom location
            string page = Controller.SendCommand("albite_getPageForLocation", args);

            // finally, simply go to this page
            goToPage(int.Parse(page));
        }

        private void goToPage(int pageNumber)
        {
            pageNumber = validatePageNumber(pageNumber);
            Controller.SendCommand("albite_goToPage", new string[] { pageNumber.ToString() });
            currentPage = pageNumber;
        }
        #endregion

        #region Browser Navigation

        private void reloadBrowser()
        {
            if (Controller.SourceUri != mainUri)
            {
                return;
            }

            // Get the current location
            DomLocation location = DomLocation;

            // reload
            navigateBrowser();

            // Set to the same location
            DomLocation = location;
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
            mainPageTemplate.Debug = true;
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
            baseStylesTemplate.ControlBackground = settings.ControlBackground;
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

            Controller.LoadingStarted();
            updateDimensions(viewportWidth, viewportHeight);
            reloadBrowser();
        }

        private void updateDimensions(int viewportWidth, int viewportHeight)
        {
            Log.D(tag, string.Format("UpdateDimensions: {0}x{1}", viewportWidth, viewportHeight));

            int width = viewportWidth;
            int height = viewportHeight;
            mainPageTemplate.FullPageWidth = width;
            mainPageTemplate.ViewportWidth = viewportWidth;
            mainPageTemplate.SaveToStorage();

            int viewportReference = Math.Max(width, height);
            int marginLeft = (int) (settings.MarginLeft * viewportReference);
            int marginRight = (int) (settings.MarginRight * viewportReference);
            int marginTop = (int) (settings.MarginTop * viewportReference);
            int marginBottom = (int) (settings.MarginBottom * viewportReference);

            baseStylesTemplate.MarginTop = marginTop;
            baseStylesTemplate.MarginBottom = marginBottom;
            baseStylesTemplate.MarginLeft = marginLeft;
            baseStylesTemplate.MarginRight = marginRight;

            int pageWidth = width - (marginLeft + marginRight);
            int pageHeight = height - (marginTop + marginBottom);

            baseStylesTemplate.ViewportWidth = viewportWidth;
            baseStylesTemplate.PageWidth = pageWidth;
            baseStylesTemplate.PageHeight = pageHeight;

            baseStylesTemplate.SaveToStorage();

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

        #region Handling Command from the JS Client

        private static readonly string debugCommand = "{debug}";
        private static readonly string errorCommand = "{error}";
        private static readonly string loadedCommand = "{loaded}";

        public void ReceiveCommand(string command)
        {
            if (command.StartsWith(loadedCommand))
            {
                handleLoadedCommand();
            }
            else if (command.StartsWith(debugCommand))
            {
                handleDebugCommand(command.Substring(debugCommand.Length));
            }
            else if (command.StartsWith(errorCommand))
            {
                handleErrorCommand(command.Substring(errorCommand.Length));
            }
            else
            {
                Controller.OnError("Unknown command: " + command);
            }
        }

        private void handleLoadedCommand()
        {
            // Inform the EngineController that it's ready
            Controller.LoadingCompleted();

            // Now get the page count
            PageCount = int.Parse(Controller.SendCommand("albite_getPageCount"));

            // Get the current page number
            currentPage = int.Parse(Controller.SendCommand("albite_getCurrentPageNumber"));

            // Handle missed orientations
            UpdateDimensions();
        }

        private void handleDebugCommand(string message)
        {
            Log.I(tag, "JavaScript: " + message);
        }

        private void handleErrorCommand(string message)
        {
            Controller.OnError("JavaScript Error: " + message);
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

            string SendCommand(string command);
            string SendCommand(string command, string[] args);

            Book.Presenter Presenter { get; }

            bool IsLoading { get; }
            void LoadingStarted();
            void LoadingCompleted();

            void OnError(string message);
        }
        #endregion
    }
}
