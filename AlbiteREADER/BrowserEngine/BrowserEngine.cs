using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.BrowserEngine
{
    public class BrowserEngine
    {
        private readonly WebBrowser webBrowser;
        // The settings are read-only, because their values will be updated
        // through data binding.
        private readonly Settings settings;
        public Settings Settings
        {
            get
            {
                return settings;
            }
        }

        private readonly TemplateResource mainPageTemplate;
        private readonly TemplateResource baseStylesTemplate;
        private readonly TemplateResource contentStylesTemplate;
        private readonly TemplateResource themeStylesTemplate;

        public BrowserEngine(WebBrowser webBrowser, Settings settings)
        {
            this.webBrowser = webBrowser;
            this.settings = settings;
        }

        private BrowserEngine()
        {
            // First, copy the JSEngine to the Isolated Storage
            {
                string filename = Paths.JSEngine;
                using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
                {
                    using (AlbiteResourceStorage res = new AlbiteResourceStorage(filename))
                    {
                        res.CopyTo(iso);
                    }
                }
            }

            // Then load the templates
            mainPageTemplate = new TemplateResource(Paths.MainPage);
            baseStylesTemplate = new TemplateResource(Paths.BaseStyles);
            contentStylesTemplate = new TemplateResource(Paths.ContentStyles);
            themeStylesTemplate = new TemplateResource(Paths.ThemeStyles);
        }

        /// <summary>
        /// Called whenever the viewport is resized and/or the margins
        /// have been changed.
        /// </summary>
        private void updateDimensions()
        {
            int fullPageWidth = (int) webBrowser.ActualWidth;
            int fullPageHeight = (int) webBrowser.ActualHeight;
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

            baseStylesTemplate["page_margin_top"] = settings.MarginTop.ToString();
            baseStylesTemplate["page_margin_bottom"] = settings.MarginBottom.ToString();
            baseStylesTemplate["page_margin_left"] = settings.MarginLeft.ToString();
            baseStylesTemplate["page_margin_right"] = settings.MarginRight.ToString();

            // Don't forget to update the dimensions as well as they
            // depend on the margins.
            updateDimensions();
        }

        private Chapter chapter;

        /// <summary>
        /// Setting this property would cause the engine
        /// to load into the last reading location in the chapter
        /// </summary>
        public Chapter Chapter
        {
            get
            {
                return chapter;
            }

            set
            {
                chapter = value;
                open();
            }
        }

        private void open()
        {
            // Set up the storage
            
            
            apply();
        }

        private void apply()
        {
            updateChapterLocation();
            reflow();
        }

        /// <summary>
        /// This should be called when the content needs
        /// to be reflowed, e.g. after changing the settings, etc.
        /// </summary>
        private void reflow()
        {
        }

        private void updateChapterLocation()
        {
            //TODO: Write JScript code that will tell the current reading location
            //      using ScriptNotify() and window.external.notify().
        }

        public void goToLocation(Location location)
        {
            //TODO: Tell the JSEngine to go to this location.
        }

        public void goToFirstPage()
        {
            //TODO
        }

        public void goToLastPage()
        {
            //TODO
        }
    }
}
