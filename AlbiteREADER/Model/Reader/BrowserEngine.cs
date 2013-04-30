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
    public class BrowserEngine : IDisposable
    {
        private static readonly string tag = "BrowserEngine";

        protected readonly WebBrowser Browser;
        protected readonly Book.Presenter Presenter;

        private readonly ILoader loader;

        private bool loading = true;

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

        public BrowserEngine(WebBrowser webBrowser, ILoader loader, Book.Presenter presenter, Settings settings)
        {
            this.Browser = webBrowser;
            this.loader = loader;
            this.Presenter = presenter;
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
                startLoading();

                chapter = value;

                // Set up the main.xhtml
                mainPageTemplate["chapter_title"] = "untitled"; //ToDo
                mainPageTemplate["chapter_file"] = Path.Combine("/" + Presenter.RelativeContentPath, chapter.Url);
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
            browserPrepare();

            mainUri = new Uri(Path.Combine(Presenter.RelativeEnginePath, Paths.MainPage), UriKind.Relative);

            string enginePath = Presenter.EnginePath;

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
        private void updateDimensions(int width, int height)
        {
            int pageWidth = width - (settings.MarginLeft + settings.MarginRight);
            int pageHeight = height - (settings.MarginTop + settings.MarginBottom);

            mainPageTemplate["full_page_width"] = width.ToString();
            mainPageTemplate.SaveToStorage();

            baseStylesTemplate["page_width_x_3"] = (width * 3).ToString();
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
            updateDimensions((int) Browser.ActualWidth, (int) Browser.ActualHeight);
        }

        private void reloadBrowser()
        {
            if (Browser.Source != mainUri)
            {
                return;
            }

            // TODO: Get the location
            navigateBrowser();
            // TODO: Set the location
        }

        private void navigateBrowser()
        {
            Browser.Navigate(mainUri);
        }

        private void goToLocation(DomLocation location)
        {
            //TODO: Tell the JSEngine to go to this location.
        }

        private void goToPage(int pageNumber)
        {
            //TODO
        }

        public void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            // x is inverted in JavaScript
            int x = (int) -e.ManipulationOrigin.X;
            int y = (int) e.ManipulationOrigin.Y;

            sendCommand("albite_press", new string[] { x.ToString(), y.ToString() });
        }

        public void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            // x is inverted in JavaScript
            int x = (int) -e.DeltaManipulation.Translation.X;
            int y = (int) e.DeltaManipulation.Translation.Y;

            sendCommand("albite_move", new string[] { x.ToString(), y.ToString() });
        }

        public void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            // x is inverted in JavaScript
            int x = (int) -e.TotalManipulation.Translation.X;
            int y = (int) e.TotalManipulation.Translation.Y;
            int velocityX = (int) -e.FinalVelocities.LinearVelocity.X;

            sendCommand("albite_release", new string[] { x.ToString(), y.ToString() });
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            Log.D(tag, "Navigated: " + e.Uri.ToString() + ", initiator: " + e.IsNavigationInitiator
                            + ", mode: " + e.NavigationMode);
        }

        private void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            startLoading();
            Log.D(tag, "Navigating to: " + e.Uri.ToString());
        }

        private void browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Log.E(tag, "Navigation failed: " + e.Uri.ToString());
            e.Handled = true;
        }

        private void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            processCommand(e.Value);
        }

        private void browser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log.D(tag, "Size changed: " + e.NewSize.Width + " x " + e.NewSize.Height);
            startLoading();
            updateDimensions((int) e.NewSize.Width, (int) e.NewSize.Height);
        }

        private EventHandler<NavigationEventArgs> browser_NavigatedHandler;
        private EventHandler<NavigatingEventArgs> browser_NavigatingHandler;
        private NavigationFailedEventHandler browser_NavigationFailedHandler;
        private EventHandler<NotifyEventArgs> browser_ScriptNotifyHandler;
        private SizeChangedEventHandler browser_SizeChangedHandler;

        private void browserPrepare()
        {
            browser_NavigatedHandler = new EventHandler<NavigationEventArgs>(browser_Navigated);
            browser_NavigatingHandler = new EventHandler<NavigatingEventArgs>(browser_Navigating);
            browser_NavigationFailedHandler = new NavigationFailedEventHandler(browser_NavigationFailed);
            browser_ScriptNotifyHandler = new EventHandler<NotifyEventArgs>(browser_ScriptNotify);
            browser_SizeChangedHandler = new SizeChangedEventHandler(browser_SizeChanged);

            Browser.Navigated += browser_NavigatedHandler;
            Browser.Navigating += browser_NavigatingHandler;
            Browser.NavigationFailed += browser_NavigationFailedHandler;
            Browser.ScriptNotify += browser_ScriptNotifyHandler;
            Browser.SizeChanged += browser_SizeChangedHandler;

            Browser.Base = Presenter.Path;
        }

        private void browserRelease()
        {
            if (browser_NavigatedHandler != null)
            {
                Browser.Navigated -= browser_NavigatedHandler;
                browser_NavigatedHandler = null;
            }

            if (browser_NavigatingHandler != null)
            {
                Browser.Navigating -= browser_NavigatingHandler;
                browser_NavigatingHandler = null;
            }

            if (browser_NavigationFailedHandler != null)
            {
                Browser.NavigationFailed -= browser_NavigationFailedHandler;
                browser_NavigationFailedHandler = null;
            }

            if (browser_ScriptNotifyHandler != null)
            {
                Browser.ScriptNotify -= browser_ScriptNotifyHandler;
                browser_ScriptNotifyHandler = null;
            }

            if (browser_SizeChangedHandler != null)
            {
                Browser.SizeChanged -= browser_SizeChangedHandler;
                browser_SizeChangedHandler = null;
            }
        }

        private object sendCommand(string command, string[] args)
        {
            Log.D(tag, "sendcommand: " + command);

            if (loading)
            {
                Log.D(tag, "Can't send command. Still loading.");
                return null;
            }

            return Browser.InvokeScript(command, args);
        }

        private static readonly string debugCommand = "{debug}";
        private static readonly string loadedCommand = "{loaded}";

        private void processCommand(string command)
        {
            if (command.StartsWith(loadedCommand))
            {
                completeLoading();
            }
            else if (command.StartsWith(debugCommand))
            {
                Log.I(tag, "JavaScript: " + command.Substring(debugCommand.Length));
            }
            else
            {
                Log.E(tag, "Unknown command: " + command);
            }
        }

        public void Dispose()
        {
            Presenter.Dispose();
            browserRelease();
        }

        private void startLoading()
        {
            loading = true;
            loader.LoadingStarted();
        }

        private void completeLoading()
        {
            loader.LoadingCompleted();
            loading = false;
        }

        public interface ILoader
        {
            void LoadingStarted();
            void LoadingCompleted();
        }
    }
}
