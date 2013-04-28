using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;
using System.Reflection;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public class BrowserEngine
    {
        private readonly WebBrowser webBrowser;

        private readonly Book.Presenter presenter;

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

        public BrowserEngine(WebBrowser webBrowser, Book.Presenter presenter, Settings settings)
        {
            this.webBrowser = webBrowser;
            this.presenter = presenter;
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
                chapter = value;

                // Set up the main.xhtml
                mainPageTemplate["chapter_title"] = "untitled"; //ToDo
                mainPageTemplate["chapter_file"] = Path.Combine("/" + presenter.RelativeContentPath, chapter.Url);
                mainPageTemplate.SaveToStorage();

                // Now navigate the web browser
                webBrowser.Source = mainUri;
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

        // Private API

        private void prepare()
        {
            // Set up the WebBrowser
            webBrowser.Base = presenter.Path;
            mainUri = new Uri(Path.Combine(presenter.RelativeEnginePath, Paths.MainPage), UriKind.Relative);

            string enginePath = presenter.EnginePath;

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
        private void updateDimensions()
        {
            int fullPageWidth = (int)webBrowser.ActualWidth;
            int fullPageHeight = (int)webBrowser.ActualHeight;
            int pageWidth = fullPageWidth - (settings.MarginLeft + settings.MarginRight);
            int pageHeight = fullPageHeight - (settings.MarginTop + settings.MarginBottom);

            mainPageTemplate["full_page_width"] = fullPageWidth.ToString();
            mainPageTemplate.SaveToStorage();

            baseStylesTemplate["page_width_x_3"] = (fullPageWidth * 3).ToString();
            baseStylesTemplate["page_width"] = pageWidth.ToString();
            baseStylesTemplate["page_height"] = pageHeight.ToString();
            baseStylesTemplate.SaveToStorage();
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
            // depend on the margins.
            updateDimensions();
        }

        private void goToLocation(DomLocation location)
        {
            //TODO: Tell the JSEngine to go to this location.
        }

        private void goToPage(int pageNumber)
        {
            //TODO
        }
    }
}
