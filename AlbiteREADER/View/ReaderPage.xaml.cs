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
using Microsoft.Phone.Controls;
using System.Diagnostics;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER.Model.Reader.Browser;
using SvetlinAnkov.Albite.READER.Utils;

namespace SvetlinAnkov.Albite.READER.View
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        private static readonly string tag = "ReaderPage";

        private int bookId;

        public ReaderPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) { }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the book id from the query string
            bookId = int.Parse(NavigationContext.QueryString["id"]);
        }

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log.D(tag, "Navigated: " + e.Uri.ToString() + ", initiator: " + e.IsNavigationInitiator
                            + ", mode: " + e.NavigationMode);
        }

        private void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            Log.D(tag, "Navigating to: " + e.Uri.ToString());
        }

        private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Log.E(tag, "Navigation failed: " + e.Uri.ToString());
            e.Handled = true;
        }

        private void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Log.D(tag, "ScriptNotify: " + e.Value);
        }

        private void Browser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log.D(tag, "Size changed: " + e.NewSize.Width + " x " + e.NewSize.Height);
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "Browser Loaded");

            openBook();
        }

        private void Browser_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "Browser Unloaded");

            closeBook();
        }

        // Implementation
        private Book.Presenter presenter;
        private BookEngine engine;

        // TODO: Add proper thread synchronization

        private void openBook()
        {
            // TODO: The following needs to be run on
            // a different thread while a loading
            // symbol is being shown.

            // Get the library from the current context
            Library library = App.Context.Library;

            // Get the book for the given id
            Book book = library.Books[bookId];

            // Get the presenter
            presenter = library.Books.GetPresenter(book);

            // Load the engine
            engine = new BookEngine(Browser, presenter, Defaults.Layout.DefaultSettings);

            // Go to the last reading location
            engine.BookLocation = presenter.BookLocation;
        }

        private void closeBook()
        {
            presenter.Dispose();
            presenter = null;
        }
    }
}