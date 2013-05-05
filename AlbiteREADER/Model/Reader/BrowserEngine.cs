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
        private TemplateResource themeStylesTemplate;

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

        /// <summary>
        /// Gets / sets the current page
        /// </summary>
        public int Page
        {
            get
            {
                // TODO
                return 0;
            }

            set { goToPage(value); }
        }

        public int PageCount
        {
            get { return 0; /*TODO*/ }
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

            // Set defaults. These will be overwritten upon open
            mainPageTemplate["chapter_title"] = "";
            mainPageTemplate["chapter_file"] = "";
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

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(
                Path.Combine(Paths.BasePath, Paths.ThemeStyles), assemblyName))
            {
                themeStylesTemplate = new TemplateResource(
                    res, Path.Combine(enginePath, Paths.ThemeStyles));
            }

            // Set up the templates
            updateLayout();
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
        }

        private void goToPage(int pageNumber)
        {
            //TODO
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
            else
            {
                Log.E(tag, "Unknown command: " + command);
            }
        }

        private void handleLoadedCommand()
        {
            // Inform the EngineController that it's ready
            Controller.LoadingCompleted();

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
        }
    }
}
