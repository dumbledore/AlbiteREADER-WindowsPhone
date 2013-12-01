using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Engine;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public partial class ReaderControl : UserControl
    {
        private static readonly string tag = "ReaderControl";

        public IReaderControlObserver Observer { get; set; }

        private readonly EnginePresenter presenter;

        public ReaderControl()
        {
            InitializeComponent();
            presenter = new EnginePresenter(this);
            load();
        }

        #region LifeCycle
        private void load()
        {
        }

        private void unload()
        {
            // Call LoadingCompleted so to hide the popup
            // and cancel the animation.
            presenter.LoadingCompleted();
        }
        #endregion

        #region UI Events
        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "WebBrowser Loaded");
            load();
        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "WebBrowser Unloaded");
            unload();
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log.D(tag, "Navigated: " + e.Uri.ToString());
        }

        private void WebBrowser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            Log.D(tag, "Navigating to: " + e.Uri.ToString());

            // Allow only the Engine Uri to be navigated to.
            if (presenter.Engine.Uri != e.Uri)
            {
                Log.D(tag, "Cancelling navigation");
                e.Cancel = true;
                return;
            }
        }

        private void WebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            // TODO: How should the user be informed and/or
            // what has to be done? This a fault of the app, not the epub.
            // TODO: Handling failed navigation in the iframe?
            // What about errors from the client?
            // What about an '{loadingError}' event from the client?
            Log.E(tag, "Navigation failed: " + e.Uri.ToString());
            e.Handled = true;
        }

        private void WebBrowser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            try
            {
                presenter.Engine.ReceiveMessage(e.Value);
            }
            catch (Exception ex)
            {
                presenter.OnError(ex.Message);
            }
        }

        public void WebBrowser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // The SizeChanged event might come before the book has been opened
            if (presenter.Engine != null)
            {
                Log.D(tag, "SizeChanged: " + e.NewSize.Width + "x" + e.NewSize.Height);
                presenter.Engine.UpdateDimensions();
            }
        }
        #endregion

        #region Public API
        public void OpenBook(Book book)
        {
            presenter.OpenBook(book);
        }

        public void PersistBook()
        {
            presenter.PersistBook();
        }

        public BookLocation BookLocation
        {
            get { return presenter.BookLocation; }
            set { presenter.BookLocation = value; }
        }

        public BookPresenter BookPresenter
        {
            get { return presenter.BookPresenter; }
        }
        #endregion

        #region EnginePresenter
        private class EnginePresenter : IEnginePresenter
        {
            private readonly ReaderControl control;

            private BookPresenter bookPresenter;
            private AlbiteEngine engine;

            public EnginePresenter(ReaderControl control)
            {
                this.control = control;
            }

            public AlbiteEngine Engine
            {
                get { return engine; }
            }

            public BookLocation BookLocation
            {
                get { return Engine.Navigator.BookLocation; }
                set { Engine.Navigator.BookLocation = value; }
            }

            public int Width
            {
                get { return (int) control.WebBrowser.ActualWidth; }
            }

            public int Height
            {
                get { return (int) control.WebBrowser.ActualHeight; }
            }

            public string BasePath
            {
                get { return control.WebBrowser.Base; }
                set { control.WebBrowser.Base = value; }
            }

            public void ReloadBrowser()
            {
                control.WebBrowser.Navigate(Engine.Uri);
            }

            public BookPresenter BookPresenter
            {
                get { return bookPresenter; }
            }

            public void OpenBook(Book book)
            {
                // Get the presenter
                bookPresenter = new BookPresenter(book);

                // TODO: The component background color needs to be data-bound
                //       to Resources["PhoneBackgroundBrush"]

                // Load the engine.
                // This will initialise the settings with their default values.
                engine = new AlbiteEngine(this, bookPresenter, new Settings());

                // Go to the last reading location
                engine.Navigator.BookLocation = bookPresenter.BookLocation;
            }

            public void PersistBook()
            {
                if (!Engine.IsLoading)
                {
                    // Can't get a book location when loading, so:
                    // 1. If it's the first load, nothing to persist anyway
                    // 2. Persist the location elsewhere before calling ReloadBrowser()
                    // True, real jumps would actually miss the latest location,
                    // e.g. when trying to persist while loading the next chapter,
                    // but that's the only problem with this approach and I don't
                    // see a better solution.
                    // It should also work when tombstoning the app.
                    bookPresenter.BookLocation = Engine.Navigator.BookLocation;
                }

                bookPresenter.Persist();
            }

            public string SendMessage(string message)
            {
                Log.D(tag, "SendMessage: " + message);

                if (Engine.IsLoading)
                {
                    Log.D(tag, "Can't send command. Still loading.");
                    return null;
                }

                return (string)control.WebBrowser.InvokeScript(
                    "albite_notify", new string[] { message });
            }

            public void LoadingStarted()
            {
                if (control.Observer != null)
                {
                    control.Observer.OnContentLoadingStarted();
                }
            }

            public void LoadingCompleted()
            {
                if (control.Observer != null)
                {
                    control.Observer.OnContentLoadingCompleted();
                }
            }

            public bool NavigationRequested(Uri uri)
            {
                if (control.Observer != null)
                {
                    return control.Observer.OnNavigationRequested(uri);
                }

                return false;
            }

            public void Navigating(BookLocation currentLocation)
            {
                if (control.Observer != null)
                {
                    control.Observer.OnNavigating(currentLocation);
                }
            }

            public void OnError(string message)
            {
                Log.E(tag, "ReaderError: " + message);

                if (control.Observer != null)
                {
                    control.Observer.OnError(message);
                }
            }

            public int ApplicationBarHeight
            {
                get
                {
                    if (control.Observer != null)
                    {
                        return control.Observer.ApplicationBarHeight;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        #endregion
    }
}
