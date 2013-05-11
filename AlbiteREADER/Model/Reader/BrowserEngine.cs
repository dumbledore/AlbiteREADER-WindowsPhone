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
    public class BrowserEngine
    {
        private static readonly string tag = "BrowserEngine";

        protected readonly IEngineController Controller;

        private Uri mainUri;

        // The settings are read-only, because their values will be updated
        // through data binding.
        private readonly Settings settings;
        public Settings Settings
        {
            get { return settings; }
        }

        private TemplateResource mainPageTemplate;
        private TemplateResource baseStylesTemplate;
        private TemplateResource contentStylesTemplate;

        public BrowserEngine(IEngineController controller, Settings settings)
        {
            this.Controller = controller;
            this.settings = settings;

            prepare();
        }

        private static readonly string assemblyName;

        static BrowserEngine()
        {
            AssemblyName name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            assemblyName = name.Name;
        }

        private Chapter chapter;
        public Chapter Chapter
        {
            get { return chapter; }

            set
            {
                Controller.LoadingStarted();

                chapter = value;

                // Set up the main.xhtml
                mainPageTemplate["chapter_title"] = "untitled"; //ToDo
                mainPageTemplate["chapter_file"] = Path.Combine("/" + Controller.Presenter.RelativeContentPath, chapter.Url);
                mainPageTemplate.SaveToStorage();

                // Now navigate the web browser
                navigateBrowser();
            }
        }

        public DomLocation DomLocation
        {
            get
            {
                //TODO: Write JScript code that will tell the current reading location
                //      using ScriptNotify() and window.external.notify().
                return null;
            }

            set { goToLocation(value); }
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

        //Note: there are always AT LEAST 3 pages
        public int FirstPageNumber { get { return 1; } }
        public int LastPageNumber { get { return PageCount - 2; } }

        public bool IsFirstPage { get { return currentPage <= FirstPageNumber; } }
        public bool IsLastPage { get { return currentPage >= LastPageNumber; } }

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

        private void prepare()
        {
            Controller.BasePath = Controller.Presenter.Path;

            mainUri = new Uri(Path.Combine(Controller.Presenter.RelativeEnginePath, Paths.MainPage), UriKind.Relative);

            string enginePath = Controller.Presenter.EnginePath;

            // Copy the JSEngine to the Isolated Storage
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(
                Path.Combine(enginePath, Paths.JSEngine)))
            {
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(
                    Path.Combine(Paths.BasePath, Paths.JSEngine), assemblyName))
                {
                    res.CopyTo(iso);
                }
            }

            // Load the templates
            using (AlbiteResourceStorage res = new AlbiteResourceStorage(
                Path.Combine(Paths.BasePath, Paths.MainPage), assemblyName))
            {
                mainPageTemplate = new TemplateResource(
                    res, Path.Combine(enginePath, Paths.MainPage));
            }

#if DEBUG
            mainPageTemplate["debug"] = "true";
#else
            mainPageTemplate["debug"] = "false";
#endif

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(
                Path.Combine(Paths.BasePath, Paths.BaseStyles), assemblyName))
            {
                baseStylesTemplate = new TemplateResource(
                    res, Path.Combine(enginePath, Paths.BaseStyles));
            }

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(
                Path.Combine(Paths.BasePath, Paths.ContentStyles), assemblyName))
            {
                contentStylesTemplate = new TemplateResource(
                    res, Path.Combine(enginePath, Paths.ContentStyles));
            }

            // Set up the templates
            updateLayout();

            // Set up the theme
            updateTheme();
        }

        private void updateTheme()
        {
            baseStylesTemplate["control_background"] = settings.ControlBackground;
            baseStylesTemplate["background_color"] = settings.Theme.BackgroundColor;
            baseStylesTemplate["text_color"] = settings.Theme.FontColor;
            baseStylesTemplate.SaveToStorage();

            contentStylesTemplate["background_color"] = settings.Theme.BackgroundColor;
            contentStylesTemplate["text_color"] = settings.Theme.FontColor;
            contentStylesTemplate["accent_color"] = settings.Theme.AccentColor;
            contentStylesTemplate.SaveToStorage();
        }

        /// <summary>
        /// Called whenever the viewport is resized and/or the margins
        /// have been changed.
        /// </summary>
        public void UpdateDimensions(int viewportWidth, int viewportHeight)
        {
            Log.D(tag, string.Format("UpdateDimensions: {0}x{1}", viewportWidth, viewportHeight));

            Controller.LoadingStarted();

            int width = viewportWidth / 3;
            int pageWidth = width - (settings.MarginLeft + settings.MarginRight);
            int pageHeight = viewportHeight - (settings.MarginTop + settings.MarginBottom);

            mainPageTemplate["full_page_width"] = width.ToString();
            mainPageTemplate["viewport_width"] = viewportWidth.ToString();
            mainPageTemplate.SaveToStorage();

            baseStylesTemplate["page_width_x_3"] = viewportWidth.ToString();
            baseStylesTemplate["page_width"] = pageWidth.ToString();
            baseStylesTemplate["page_height"] = pageHeight.ToString();
            baseStylesTemplate.SaveToStorage();

            reloadBrowser();
        }

        private void updateLayout()
        {
            contentStylesTemplate["line_height"] = settings.LineHeight.ToString();
            contentStylesTemplate["font_size"] = settings.FontSize.ToString();
            contentStylesTemplate["font_family"] = settings.FontFamily;
            contentStylesTemplate["text_align"] = settings.TextAlign.ToString();
            contentStylesTemplate.SaveToStorage();

            baseStylesTemplate["page_margin_top"] = settings.MarginTop.ToString();
            baseStylesTemplate["page_margin_bottom"] = settings.MarginBottom.ToString();
            baseStylesTemplate["page_margin_left"] = settings.MarginLeft.ToString();
            baseStylesTemplate["page_margin_right"] = settings.MarginRight.ToString();

            // Don't forget to update the dimensions as well as they
            // depend on the margins. This will reload the browser.
            UpdateDimensions((int) Controller.ViewportWidth, (int) Controller.ViewportHeight);
        }

        private void reloadBrowser()
        {
            if (Controller.SourceUri != mainUri)
            {
                return;
            }

            // TODO: Get the location
            navigateBrowser();
            // TODO: Set the location
        }

        private void navigateBrowser()
        {
            Controller.SourceUri = mainUri;
        }

        private DomLocation locationCached;

        private void goToLocation(DomLocation location)
        {
            Log.D(tag, string.Format("Going to location #{0}/{1}",
                location.ElementIndex, location.TextOffset));

            // clear the cached value
            locationCached = null;

            if (Controller.IsLoading)
            {
                Log.D(tag, "Still loading. Cache the location");
                locationCached = location;
                return;
            }

            //TODO: Tell the JSEngine to go to this location.
            goToPage(10);
        }

        private void goToPage(int pageNumber)
        {
            Log.D(tag, "Going to page #" + pageNumber);

            pageNumber = validatePageNumber(pageNumber);

            // This will update the current page to have the same content as the
            // page going to
            Controller.SendCommand("albite_goToPage1", new string[] { pageNumber.ToString() });

            // Wait for the browser to keep up with rendering so that there wouldn't
            // be any visible tearing.
            // Unfortunately, there doesn't seem to be any better way of doing this
            // on WindowsPhone 7.5
            Thread.Sleep(150);

            // Reset position to current page
            Controller.ResetScrollPosition();

            // Now update the previous/next pages as well.
            Controller.SendCommand("albite_goToPage2");

            currentPage = pageNumber;
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

            // And finally, go to the location if there was one
            if (locationCached != null)
            {
                goToLocation(locationCached);
            }
        }

        private void handleDebugCommand(string message)
        {
            Log.I(tag, "JavaScript: " + message);
        }

        private void handleErrorCommand(string message)
        {
            Controller.OnError("JavaScript Error: " + message);
        }

        public interface IEngineController
        {
            double ViewportWidth { get; }
            double ViewportHeight { get; }

            string BasePath { get; set; }
            Uri SourceUri { get; set; }

            void ResetScrollPosition();

            string SendCommand(string command);
            string SendCommand(string command, string[] args);

            Book.Presenter Presenter { get; }

            bool IsLoading { get; }
            void LoadingStarted();
            void LoadingCompleted();

            void OnError(string message);
        }
    }
}
