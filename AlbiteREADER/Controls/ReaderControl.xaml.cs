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

        public static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register("ScrollOffset", typeof(double), typeof(ReaderControl),
            new PropertyMetadata(OnScrollOffsetChanged));

        public double ScrollOffset
        {
            get { return (double)GetValue(ScrollOffsetProperty); }
            set { SetValue(ScrollOffsetProperty, value); }
        }

        private static void OnScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReaderControl readerControl = (ReaderControl) d;
            double offset = (double) e.NewValue;
            readerControl.ScrollViewer.ScrollToHorizontalOffset(offset);
        }

        private EngineController controller;

        public ReaderControl()
        {
            InitializeComponent();
            load();
        }

        #region UI Events
        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WebBrowser.Width = e.NewSize.Width * 3;
        }

        private void ScrollViewer_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            logEvent(string.Format(
                "ManipulationStarted: ({0}, {1})", e.ManipulationOrigin.X, e.ManipulationOrigin.Y));
        }

        private void ScrollViewer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            logEvent(string.Format(
                "ManipulationDelta: ({0}, {1})",
                e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y));
        }

        private void ScrollViewer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            logEvent(string.Format(
                "ManipulationCompleted: ({0}, {1})",
                e.TotalManipulation.Translation.X, e.TotalManipulation.Translation.Y));
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            logEvent("Loaded");
            load();
        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            logEvent("Unloaded");
            unload();
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            logEvent("Navigated: " + e.Uri.ToString());
        }

        private void WebBrowser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            logEvent("Navigating to: " + e.Uri.ToString());

            if (controller == null || controller.Engine == null)
            {
                return;
            }

            if (controller.Engine.NavigateTo(e.Uri))
            {
                // Handled internally, has to cancel
                logEvent("Cancelling navigation");
                e.Cancel = true;
            }

            controller.LoadingStarted();
        }

        private void WebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Log.E(tag, "Navigation failed: " + e.Uri.ToString());
            e.Handled = true;
        }

        private void WebBrowser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            logEvent("ScriptNotify: " + e.Value);

            if (controller == null || controller.Engine == null)
            {
                return;
            }

            controller.Engine.ReceiveCommand(e.Value);
        }

        public void WebBrowser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            logEvent("SizeChanged: " + e.NewSize.Width + "x" + e.NewSize.Height);

            if (controller == null || controller.Engine == null)
            {
                return;
            }

            controller.Engine.UpdateDimensions(
                (int) e.NewSize.Width, (int) e.NewSize.Height);
        }

        #endregion

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

            controller.LoadingCompleted();
            controller.Dispose();
            controller = null;
        }

        public void OpenBook(int bookId)
        {
            controller.OpenBook(bookId);
        }

        public void CloseBook()
        {
            controller.CloseBook();
        }

        public void Dispose()
        {
            if (controller!= null)
            {
                controller.Dispose();
            }
        }

        [Conditional("DEBUG")]
        private void logEvent(string msg)
        {
            Log.D(tag, msg + " @ " + Thread.CurrentThread.Name + " # " + Thread.CurrentThread.ManagedThreadId);
        }

        private class EngineController : BrowserEngine.IEngineController, IDisposable
        {
            private readonly ReaderControl control;
            private WaitPopup waitPopup = new WaitPopup();

            private Book.Presenter presenter;
            private BookEngine engine;

            private bool isLoading = true;

            public EngineController(ReaderControl control)
            {
                this.control = control;
            }

            public BookEngine Engine
            {
                get { return engine; }
            }

            public double ViewportWidth
            {
                get { return control.WebBrowser.ActualWidth; }
            }

            public double ViewportHeight
            {
                get { return control.WebBrowser.ActualHeight; }
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

            public object SendCommand(string command, string[] args)
            {
                Log.D(tag, "sendcommand: " + command);

                if (isLoading)
                {
                    Log.D(tag, "Can't send command. Still loading.");
                    return null;
                }

                return control.WebBrowser.InvokeScript(command, args);
            }

            public void LoadingStarted()
            {
                isLoading = true;
                waitPopup.IsOpen = true;
            }

            public void LoadingCompleted()
            {
                control.ScrollViewer.ScrollToHorizontalOffset(
                    control.ScrollViewer.ActualWidth);

                waitPopup.IsOpen = false;
            }
        }
    }
}
