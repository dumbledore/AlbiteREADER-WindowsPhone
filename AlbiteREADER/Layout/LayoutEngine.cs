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
using SvetlinAnkov.AlbiteREADER.Model;
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Layout
{
    public class LayoutEngine
    {
        private readonly WebBrowser webBrowser;
        // The settings are read-only, because their values will be updated
        // through data binding.
        private readonly LayoutSettings settings;
        public LayoutSettings Settings
        {
            get
            {
                return settings;
            }
        }

        private static readonly string FilesLocation = "/Layout/";
        private static readonly string[] FilesToCopy = { "Albite.js" };

        private readonly LayoutTemplateResource contentTemplate = new LayoutTemplateResource(FilesLocation + "Content.css");
        private readonly LayoutTemplateResource mainTemplate    = new LayoutTemplateResource(FilesLocation + "Main.xhtml");
        private readonly LayoutTemplateResource stylesTemplate  = new LayoutTemplateResource(FilesLocation + "Styles.css");
        private readonly LayoutTemplateResource themTemplate    = new LayoutTemplateResource(FilesLocation + "Theme.css");
        
        public LayoutEngine(WebBrowser webBrowser, LayoutSettings settings) : base()
        {
            this.webBrowser = webBrowser;
            this.settings = settings;

            prepareFiles();
        }

        private void prepareFiles()
        {
            foreach (string file in FilesToCopy)
            {
                string filename = FilesLocation + file;
                using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
                {
                    using (AlbiteResourceStorage res = new AlbiteResourceStorage(filename))
                    {
                        res.CopyTo(iso);
                    }
                }
            }
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

            mainTemplate["full_page_width"] = fullPageWidth.ToString();
            mainTemplate.SaveToStorage();

            stylesTemplate["page_width_x_3"] = (fullPageWidth * 3).ToString();
            stylesTemplate["page_width"] = pageWidth.ToString();
            stylesTemplate["page_height"] = pageHeight.ToString();
            stylesTemplate.SaveToStorage();
        }

        private void updateLayout()
        {
            contentTemplate["line_height"] = settings.LineHeight.ToString();
            contentTemplate["font_size"] = settings.FontSize.ToString();
            contentTemplate["font_family"] = settings.FontFamily;
            contentTemplate["text_align"] = settings.TextAlign.ToString();

            stylesTemplate["page_margin_top"] = settings.MarginTop.ToString();
            stylesTemplate["page_margin_bottom"] = settings.MarginBottom.ToString();
            stylesTemplate["page_margin_left"] = settings.MarginLeft.ToString();
            stylesTemplate["page_margin_right"] = settings.MarginRight.ToString();

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

        public void goToLocation(LayoutLocation location)
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
