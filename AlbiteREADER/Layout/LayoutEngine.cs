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

namespace SvetlinAnkov.AlbiteREADER.Layout
{
    public class LayoutEngine
    {
        private readonly WebBrowser webBrowser;

        public LayoutEngine(WebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;

        }

        /// <summary>
        /// The settings are read-only, because their values will be updated
        /// through data binding.
        /// </summary>
        public LayoutSettings Settings { get; private set; }
        
        public LayoutEngine(LayoutSettings settings) : base()
        {
            Settings = settings;
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

            // Only need to reflow
            reflow();
        }

        private void applyNewSettings()
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
