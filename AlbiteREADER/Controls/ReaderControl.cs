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

namespace SvetlinAnkov.Albite.READER.Controls
{
    public class ReaderControl : Control
    {
        private static readonly string tag = "ReaderControl";

        // Controls from the ControlTemplate
        private WebBrowser webBrowser;

        // Related to the Model and the Engine
        private Book.Presenter presenter;
        private BookEngine engine;

        // State
        private bool initialised = false;

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

            webBrowser.Navigated += new EventHandler<NavigationEventArgs>(webBrowser_Navigated);
            webBrowser.Navigating += new EventHandler<NavigatingEventArgs>(webBrowser_Navigating);
            webBrowser.NavigationFailed += new NavigationFailedEventHandler(webBrowser_NavigationFailed);
            webBrowser.ScriptNotify += new EventHandler<NotifyEventArgs>(webBrowser_ScriptNotify);
            webBrowser.SizeChanged += new SizeChangedEventHandler(webBrowser_SizeChanged);
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
            presenter = library.Books.GetPresenter(book);

            // Load the engine
            engine = new BookEngine(webBrowser, presenter, Defaults.Layout.DefaultSettings);

            // Go to the last reading location
            engine.BookLocation = presenter.BookLocation;
        }

        public void CloseBook()
        {
            if (presenter != null)
            {
                // TODO: Book persistance

                presenter.Dispose();
            }

            presenter = null;
            engine = null;
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            initialised = true;

            Log.D(tag, "Loaded");
        }

        private void unloaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "Unloaded");
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Log.D(tag, "Navigated: " + e.Uri.ToString() + ", initiator: " + e.IsNavigationInitiator
                            + ", mode: " + e.NavigationMode);
        }

        private void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            Log.D(tag, "Navigating to: " + e.Uri.ToString());
        }

        private void webBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Log.E(tag, "Navigation failed: " + e.Uri.ToString());
            e.Handled = true;
        }

        private void webBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Log.D(tag, "ScriptNotify: " + e.Value);
        }

        private void webBrowser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (initialised)
            {
                Log.D(tag, "Size changed: " + e.NewSize.Width + " x " + e.NewSize.Height);
            }
        }
    }
}
