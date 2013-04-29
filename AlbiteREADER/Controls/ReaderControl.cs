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
using SvetlinAnkov.Albite.Core.Utils;
using System.Windows.Navigation;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.READER.Utils;
using SvetlinAnkov.Albite.READER.Model.Reader;

namespace SvetlinAnkov.Albite.READER.Controls
{
    public class ReaderControl : Control
    {
        private static readonly string tag = "ReaderControl";

        // Controls from the ControlTemplate
        private WebBrowser webBrowser;

        // Related to the Model and the Engine
        private BookEngine engine;

        public ReaderControl()
        {
            DefaultStyleKey = typeof(ReaderControl);

            Loaded += new RoutedEventHandler(loaded);
            Unloaded += new RoutedEventHandler(unloaded);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            webBrowser = GetTemplateChild("WebBrowser") as WebBrowser;
        }

        public void OpenBook(int bookId)
        {
            // TODO: The following needs to be run on
            // a different thread while a loading
            // symbol is being shown.

            // Get the library from the current context
            Library library = App.Context.Library;

            // Get the book for the given id
            Book book = library.Books[bookId];

            // Get the presenter
            Book.Presenter presenter = library.Books.GetPresenter(book);

            // Load the engine
            engine = new BookEngine(webBrowser, presenter, Defaults.Layout.DefaultSettings);

            // Go to the last reading location
            engine.BookLocation = presenter.BookLocation;
        }

        public void CloseBook()
        {
            if (engine != null)
            {
                // TODO: Book persistance

                engine.Dispose();
            }
            engine = null;
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "Loaded");
        }

        private void unloaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "Unloaded");
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);
            engine.OnManipulationStarted(e);
            e.ManipulationContainer = this;
            e.Handled = true;
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);
            engine.OnManipulationDelta(e);
            e.Handled = true;
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            engine.OnManipulationCompleted(e);
            e.Handled = true;
        }
    }
}
