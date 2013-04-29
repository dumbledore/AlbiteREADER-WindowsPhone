using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SvetlinAnkov.Albite.READER.Model.Reader;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.READER.Utils;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.Controls
{
    public partial class ReaderControl : UserControl, IDisposable
    {
        public ReaderControl()
        {
            InitializeComponent();
        }

        private static readonly string tag = "ReaderControl";

        // Related to the Model and the Engine
        private BookEngine engine;

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
            engine = new BookEngine(WebBrowser, presenter, Defaults.Layout.DefaultSettings);

            // Go to the last reading location
            engine.BookLocation = presenter.BookLocation;
        }

        public void CloseBook()
        {
            // TODO: Book persistance
            Dispose();
        }

        public void Dispose()
        {
            if (engine != null)
            {

                engine.Dispose();
            }
            engine = null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "Loaded");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
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
