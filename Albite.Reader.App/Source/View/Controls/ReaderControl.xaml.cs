using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Engine;
using Albite.Reader.Engine.Layout;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Albite.Reader.App.View.Controls
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
        }

        #region UI Events
        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "WebBrowser Loaded");
        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "WebBrowser Unloaded");
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log.D(tag, "Navigated: " + e.Uri);
        }

        private void WebBrowser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            Log.D(tag, "Navigating to: " + e.Uri);

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
            // As WebBrowserControl is navigated to
            // an empty string whenever ReaderControl is Unloaded, i.e.
            // at a time it would not exist in the UI hierarchy,
            // it would cause a NavigationFailed event.
            // It appears that Navigating the WebBrowserControl away
            // is the only way to make sure that the JavaScript engine
            // would not be working in the background by any chance
            // after ReaderControl is unloaded or even after it's page
            // is not active any more.
            // There's nothing we can do but ignore it.
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

        public void ReaderControl_Unloaded(object sender, RoutedEventArgs e)
        {
            WebBrowser.NavigateToString("");
        }
        #endregion

        #region Public API
        public BookPresenter BookPresenter
        {
            get { return presenter.BookPresenter; }
            set { presenter.BookPresenter = value; }
        }

        public BookLocation BookLocation
        {
            get { return presenter.BookLocation; }
            set { presenter.BookLocation = value; }
        }

        public Bookmark CreateBookmark()
        {
            return presenter.CreateBookmark();
        }
        #endregion

        #region EnginePresenter
        private class EnginePresenter : IEnginePresenter
        {
            private readonly ReaderControl control;

            private BookPresenter bookPresenter;
            private IEngine engine;

            public EnginePresenter(ReaderControl control)
            {
                this.control = control;
            }

            public IEngine Engine
            {
                get { return engine; }
            }

            public BookLocation BookLocation
            {
                get
                {
                    BookLocation location = null;

                    if (Engine.IsLoading)
                    {
                        location = bookPresenter.HistoryStack.GetCurrentLocation();
                    }
                    else
                    {
                        // Can't get a book location when loading, so:
                        // 1. If it's the first load, nothing to persist anyway
                        // 2. Persist the location elsewhere before calling ReloadBrowser()
                        // True, real jumps would actually miss the latest location,
                        // e.g. when trying to persist while loading the next chapter,
                        // but that's the only problem with this approach and I don't
                        // see a better solution.
                        // It should also work when tombstoning the app.
                        location = Engine.Navigator.BookLocation;
                    }

                    return location;
                }

                set { Engine.Navigator.BookLocation = value; }
            }

            public Bookmark CreateBookmark()
            {
                return Engine.Navigator.CreateBookmark();
            }

            // HACK
            //
            // Sometimes, even though the control has fired the Loaded event
            // it would *still* return a zero ActualWidth/Height.
            //
            // This makes it impossible for the client to further lay out the content
            //
            // A working solution is to get the dimensions of the PhoneApplicationFrame.
            // This means that the control would always be expected to have the same
            // dimensions as the frame. Not that the frame dimensions do not change
            // when the page is rotated.
            //
            // For the practical use-case this is not a problem, as the ReaderPage has
            // the ReaderControl taking up the whole page.
            //
            // Of course, it would be a problem for other cases and is not a correct solution.

            public int Width
            {
                get
                {
                    PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    int width = frame.Orientation == PageOrientation.PortraitUp ? (int)frame.ActualWidth : (int)frame.ActualHeight;
                    return width;
                }
            }

            public int Height
            {
                get
                {
                    PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    int height = frame.Orientation == PageOrientation.PortraitUp ? (int)frame.ActualHeight : (int)frame.ActualWidth;
                    return height;
                }
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

                set
                {
                    // Update the current value
                    bookPresenter = value;

                    // Get the context
                    Context context = ((IApplication)App.Current).CurrentContext;

                    // Get current layout settings
                    LayoutSettings settings = context.LayoutSettings;

                    // Load the engine.
                    engine = new Albite.Reader.Engine.Engine(this, bookPresenter, settings);

                    // Go to the last reading location
                    BookLocation = bookPresenter.HistoryStack.GetCurrentLocation();
                }
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
                    Engine.NotifyFunction, new string[] { message });
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
