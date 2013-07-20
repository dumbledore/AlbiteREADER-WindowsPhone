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
using System.Diagnostics;
using System.Threading;

namespace SvetlinAnkov.Albite.READER.Controls
{
    public partial class ReaderControl : UserControl, IDisposable
    {
        private static readonly string tag = "ReaderControl";

        public event EventHandler ReaderError;

        private EngineController controller;

        private ThreadCheck threadCheck = new ThreadCheck();

        public ReaderControl()
        {
            InitializeComponent();
            load();
        }

        #region LifeCycle
        private void load()
        {
            if (controller == null)
            {
                controller = new EngineController(this);
            }

            controller.LoadingStarted();
        }

        private void unload()
        {
            if (controller == null)
            {
                return;
            }

            // Persist the book and release the files
            controller.CloseBook();

            // Call LoadingCompleted so to hide the popup
            // and cancel the animation.
            controller.LoadingCompleted();
            controller.Dispose();
            controller = null;
        }

        public void Dispose()
        {
            if (controller != null)
            {
                controller.Dispose();
            }
        }
        #endregion

        #region UI Events
        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            threadCheck.Check();

            Log.D(tag, "Loaded");
            load();
        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            threadCheck.Check();

            Log.D(tag, "Unloaded");
            unload();
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            threadCheck.Check();

            Log.D(tag, "Navigated: " + e.Uri.ToString());
        }

        private void WebBrowser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            threadCheck.Check();

            Log.D(tag, "Navigating to: " + e.Uri.ToString());

            if (controller == null || controller.Engine == null)
            {
                return;
            }

            if (controller.Engine.NavigateTo(e.Uri))
            {
                // Handled internally, has to cancel
                Log.D(tag, "Cancelling navigation");
                e.Cancel = true;
            }

            controller.LoadingStarted();
        }

        private void WebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            threadCheck.Check();

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
            threadCheck.Check();

            Log.D(tag, "ScriptNotify: " + e.Value);

            if (controller == null || controller.Engine == null)
            {
                return;
            }

            controller.Engine.ReceiveCommand(e.Value);
        }

        public void WebBrowser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            threadCheck.Check();

            Log.D(tag, "SizeChanged: " + e.NewSize.Width + "x" + e.NewSize.Height);

            if (controller == null || controller.Engine == null)
            {
                return;
            }

            controller.Engine.UpdateDimensions();
        }
        #endregion

        #region Public API
        public void OpenBook(int bookId)
        {
            threadCheck.Check();

            controller.OpenBook(bookId);
        }

        public void CloseBook()
        {
            threadCheck.Check();

            controller.CloseBook();
        }
        #endregion

        #region EngineController
        private class EngineController : BrowserEngine.IEngineController, IDisposable
        {
            private readonly ReaderControl control;
            private WaitPopup waitPopup = new WaitPopup();

            private Book.Presenter presenter;
            private BookEngine engine;

            public bool IsLoading { get; private set; }

            public EngineController(ReaderControl control)
            {
                this.control = control;
                IsLoading = true;
            }

            public BookEngine Engine
            {
                get { return engine; }
            }

            public int ViewportWidth
            {
                get { return (int) control.WebBrowser.ActualWidth; }
            }

            public int ViewportHeight
            {
                get { return (int) control.WebBrowser.ActualHeight; }
            }

            public string BasePath
            {
                get { return control.WebBrowser.Base; }
                set { control.WebBrowser.Base = value; }
            }

            public Uri SourceUri
            {
                get { return control.WebBrowser.Source; }
                set { control.WebBrowser.Navigate(value); }
            }

            public Book.Presenter Presenter
            {
                get { return presenter; }
            }

            public void OpenBook(int bookId)
            {
                // TODO: The following needs to be run on
                // a different thread perhaps?

                // Get the library from the current context
                Library library = App.Context.Library;

                // Get the book for the given id
                Book book = library.Books[bookId];

                // Get the presenter
                presenter = library.Books.GetPresenter(book);

                // TODO: The component background color needs to be data-bound
                //       to Resources["PhoneBackgroundBrush"]

                // Load the engine
                engine = new BookEngine(this, Defaults.Layout.DefaultSettings);

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
                if (presenter != null)
                {
                    presenter.Dispose();
                }

                presenter = null;
                engine = null;
            }

            public string SendCommand(string command)
            {
                return SendCommand(command, new string[0]);
            }

            public string SendCommand(string command, string[] args)
            {
                Log.D(tag, "sendcommand: " + command);

                if (IsLoading)
                {
                    Log.D(tag, "Can't send command. Still loading.");
                    return null;
                }

                return (string) control.WebBrowser.InvokeScript(command, args);
            }

            public void LoadingStarted()
            {
                //IsLoading = true;
                //waitPopup.IsOpen = true;
            }

            public void LoadingCompleted()
            {
                //waitPopup.IsOpen = false;
                //IsLoading = false;
            }

            public void OnError(string message)
            {
                Log.E(tag, "ReaderError: " + message);

                if (control.ReaderError != null)
                {
                    control.ReaderError(this, EventArgs.Empty);
                }
            }
        }
        #endregion
    }
}
