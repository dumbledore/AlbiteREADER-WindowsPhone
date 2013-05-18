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

        public ReaderControl()
        {
            InitializeComponent();
            initializeScrolling();
            initializeAnimation();
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

        private void ScrollCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updatePageProperties(e.NewSize.Width);

            WebBrowser.Width = 3 * e.NewSize.Width;
            WebBrowser.Height = e.NewSize.Height;

            ScrollBorder.Width = e.NewSize.Width;
            ScrollBorder.Height = e.NewSize.Height;
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);

            logEvent(string.Format("OnManipulationStarted: ({0}, {1})",
                e.ManipulationOrigin.X, e.ManipulationOrigin.Y));

            if (controller == null || controller.IsLoading)
            {
                Log.D(tag, "Still loading, dropping event");
                return;
            }

            if (isAnimating)
            {
                Log.D(tag, "Still animating, dropping event");
                return;
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            logEvent(string.Format("OnManipulationDelta: ({0}, {1})",
                e.DeltaManipulation.Translation.X,
                e.DeltaManipulation.Translation.Y));

            if (controller == null || controller.IsLoading)
            {
                Log.D(tag, "Still loading, dropping event");
                return;
            }

            if (isAnimating)
            {
                Log.D(tag, "Still animating, dropping event");
                return;
            }

            // Simply scroll the content
            scrollPageDelta(e.DeltaManipulation.Translation.X);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            logEvent(string.Format("OnManipulationCompleted: ({0}, {1})",
                e.TotalManipulation.Translation.X,
                e.TotalManipulation.Translation.Y));

            if (controller == null || controller.IsLoading)
            {
                Log.D(tag, "Still loading, dropping event");
                return;
            }

            if (isAnimating)
            {
                Log.D(tag, "Still animating, dropping event");
                return;
            }

            double xDelta = e.TotalManipulation.Translation.X;
            if (xDelta == 0)
            {
                double xAbs = xDelta + e.ManipulationOrigin.X;
                scrollPageTouch(xAbs);
            }
            else
            {
                scrollPageStart(xDelta, e.FinalVelocities.LinearVelocity.X);
            }
        }
        #endregion

        #region Misc
        [Conditional("DEBUG")]
        private void logEvent(string msg)
        {
            Log.D(tag, msg + " @ " + Thread.CurrentThread.Name + " # " + Thread.CurrentThread.ManagedThreadId);
        }

        private static double clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        private void onError(string message)
        {
            Log.E(tag, "ReaderError: " + message);

            if (ReaderError != null)
            {
                ReaderError(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Public API
        public void OpenBook(int bookId)
        {
            controller.OpenBook(bookId);
        }

        public void CloseBook()
        {
            controller.CloseBook();
        }
        #endregion

        #region Animation

        private DoubleAnimation scrollAnimation;
        private Storyboard scrollStoryboard;

        private void initializeAnimation()
        {
            // Create a DoubleAnimation to animate the ScrollOffset property
            scrollAnimation = new DoubleAnimation();
            scrollAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            scrollAnimation.Completed += new EventHandler(ScrollAnimation_Completed);

            // Configure the EasingFunction
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseOut;
            scrollAnimation.EasingFunction = easingFunction;

            // Configure the animation to target the ReaderControl's ScrollOffset property
            //Storyboard.SetTargetName(scrollAnimation, "ReaderRoot");
            Storyboard.SetTarget(scrollAnimation, translate);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath(TranslateTransform.XProperty));

            // Create a storyboard to contain the animation.
            scrollStoryboard = new Storyboard();
            scrollStoryboard.Children.Add(scrollAnimation);
        }

        private bool isAnimating = false;

        enum PageType
        {
            PREVIOUS_PAGE,
            CURRENT_PAGE,
            NEXT_PAGE
        };

        private PageType scrollPageType;

        private void scrollTo(PageType pageType, double speedRatio = 1.0)
        {
            double to;

            switch (pageType)
            {
                case PageType.PREVIOUS_PAGE:
                    to = previousPagePosition;
                    break;

                case PageType.CURRENT_PAGE:
                    to = currentPagePosition;
                    break;

                case PageType.NEXT_PAGE:
                    to = nextPagePosition;
                    break;

                default:
                    throw new InvalidOperationException("Bad page type");
            }

            scrollPageType = pageType;

            scrollAnimation.From = translate.X;
            scrollAnimation.To = to;
            scrollAnimation.SpeedRatio = speedRatio;

            Log.D(tag, string.Format("Scrolling from {0} to {1} in {2} msec",
                scrollAnimation.From, scrollAnimation.To,
                scrollAnimation.Duration.TimeSpan.Milliseconds / scrollAnimation.SpeedRatio));

            isAnimating = true;
            scrollStoryboard.Begin();
        }

        private void cancelScroll()
        {
            Log.D(tag, "Cancelling scroll animation");
            scrollStoryboard.Stop();
            isAnimating = false;
        }
        #endregion

        #region Page Scrolling

        private TranslateTransform translate;

        private void initializeScrolling()
        {
            translate = new TranslateTransform();
            translate.X = 0;
            translate.Y = 0;
            WebBrowser.RenderTransform = translate;
        }

        private double pageWidth;

        private double previousPagePosition;
        private double currentPagePosition;
        private double nextPagePosition;

        // If the total delta is within these bounds
        // the scroll will be back to the current page
        private static readonly double noScrollIntervalRatio = 0.075;
        private double noScrollInterval;

        // Velocity under this amount will not be
        // considered as important even if it's in
        // the opposite direction of the drag
        private static readonly double negligibleVelocityRatio = 0.075;
        private double negligibleVelocity;

        private static readonly double samePageVelocityRatio = 2.0;

        private void updatePageProperties(double pageWidth)
        {
            this.pageWidth = pageWidth;

            // Scrolling to the right is done with
            // a negative number, i.e. by dragging left

            previousPagePosition = 0;
            currentPagePosition = previousPagePosition - pageWidth;
            nextPagePosition = currentPagePosition - pageWidth;

            noScrollInterval = pageWidth * noScrollIntervalRatio;
            negligibleVelocity = pageWidth * negligibleVelocityRatio;
        }

        /// <summary>
        /// Reset the the position to the current page
        /// </summary>
        private void resetScrollPosition()
        {
            translate.X = currentPagePosition;
        }

        private void scrollPageDelta(double xDelta)
        {
            // Don't forget that: nextPagePosition < currentPagePosition < previousPagePosition
            translate.X = clamp(translate.X + xDelta, nextPagePosition, previousPagePosition);
        }

        private void scrollPageStart(double xDelta, double xVelocity)
        {
            // Scroll back to the current page if:
            //   1. The drag was too insignificant, most probably a mistake
            //   2. There's a considerable amount of velocity and it's in the direction
            //      opposite of the drag
            if (Math.Abs(xDelta) <= noScrollInterval
                || (Math.Abs(xVelocity) > negligibleVelocity && Math.Sign(xVelocity) != Math.Sign(xDelta)))
            {
                scrollTo(PageType.CURRENT_PAGE, samePageVelocityRatio);
                return;
            }

            // If xDelta is negative, then the user is dragging left
            // and therefore wants to go to the next page
            PageType pageType = xDelta < 0 ? PageType.NEXT_PAGE : PageType.PREVIOUS_PAGE;

            // The duration of the animation should reflect the current position
            double ratio = Math.Abs(pageWidth + Math.Abs(xDelta)) / pageWidth;

            // Take into account the velocity of the flick
            double velocityRatio = Math.Abs(xVelocity) / pageWidth;

            // Clamp the velocity into some sane limits. It definitely shouldn't
            // go under 1.0, i.e. shouldn't be able to slow down the animation
            // only speed it up.
            velocityRatio = clamp(velocityRatio, 1.0, 3.0);

            // Ready to scroll
            scrollTo(pageType, ratio * velocityRatio);
        }

        private void scrollPageTouch(double xAbs)
        {
            // TODO: Handle touches:
            // 1. Ask the engine if there's anything to touch (like a link)
            // 2. if not, check if ok to scroll left/right.
        }

        private void ScrollAnimation_Completed(object sender, EventArgs e)
        {
            logEvent("Animation completed");
            isAnimating = false;

            switch (scrollPageType)
            {
                case PageType.CURRENT_PAGE:
                    break;

                case PageType.PREVIOUS_PAGE:
                    if (controller.Engine.IsFirstPage)
                    {
                        if (!controller.Engine.IsFirstChapter)
                        {
                            controller.Engine.GoToPreviousChapter();
                            break;
                        }
                        else
                        {
                            // That's the first chapter.
                            scrollTo(PageType.CURRENT_PAGE);
                            break;
                        }
                    }

                    controller.Engine.Page--;
                    break;

                case PageType.NEXT_PAGE:
                    if (controller.Engine.IsLastPage)
                    {
                        if (!controller.Engine.IsLastChapter)
                        {
                            controller.Engine.GoToNextChapter();
                            break;
                        }
                        else
                        {
                            // That's the last chapter.
                            scrollTo(PageType.CURRENT_PAGE);
                            break;
                        }
                    }

                    controller.Engine.Page++;
                    break;

                default:
                    throw new InvalidOperationException("Bad page type");
            }
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

            public void ResetScrollPosition()
            {
                control.resetScrollPosition();
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
                IsLoading = true;
                waitPopup.IsOpen = true;
                control.cancelScroll();
            }

            public void LoadingCompleted()
            {
                control.cancelScroll();
                ResetScrollPosition();
                waitPopup.IsOpen = false;
                IsLoading = false;
            }

            public void OnError(string message)
            {
                control.onError(message);
            }
        }
        #endregion
    }
}
