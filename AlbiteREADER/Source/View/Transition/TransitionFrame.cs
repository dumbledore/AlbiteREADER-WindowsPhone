using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public class TransitionFrame : PhoneApplicationFrame
    {
        private INavigationTransitionFactory navigationTransitionFactory;
        private IRotationTransitionFactory rotationTransitionFactory;
        private ITransition currentTransition;
        private WriteableBitmap bitmap;
        private PageOrientation previousOrientation;

        public TransitionFrame(
            INavigationTransitionFactory navigationTransitionFactory,
            IRotationTransitionFactory rotationTransitionFactory)
        {
            this.navigationTransitionFactory = navigationTransitionFactory;
            this.rotationTransitionFactory = rotationTransitionFactory;

            // Get the initial orientation
            previousOrientation = Orientation;

            // Set up events
            Navigating += OnNavigating;
            Navigated += OnNavigated;
            OrientationChanged += OnOrientationChanged;
        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            // Stop any previous transitions
            clearTransition();

            // Clear the background brush and the cached bitmap
            clearBitmap();

            // Do we want the transition?
            if (!e.IsNavigationInitiator || !transitionEnabled())
            {
                return;
            }

            // Try caching the current screen
            try
            {
                bitmap = new WriteableBitmap(Content as UIElement, null);
            }
            catch (OutOfMemoryException)
            {
                // Not enough memory, so skip it
            }
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Stop any previous transitions
            clearTransition();

            // Do we want the transition?
            if (!e.IsNavigationInitiator || !transitionEnabled())
            {
                clearBitmap();
                return;
            }

            ImageBrush bgBrush = null;

            if (bitmap != null)
            {
                bgBrush = new ImageBrush();

                // The size of the previous page may differ
                // This will set it up so that it would look
                // correctly.
                //
                // TODO: Would this work with Panoramas?
                //
                // TODO: This doesn't work so well, actually.
                //       Better be able to turn animations off
                //       for some pages.
                //
                // Note: This works fine for long pages as
                //       it clips the hidden contents, i.e.
                //       the size is this of the page not of
                //       its contents.
                bgBrush.Stretch = Stretch.None;
                bgBrush.AlignmentY = AlignmentY.Bottom;

                // Set the bitmap as the source
                bgBrush.ImageSource = bitmap;
            }

            // Set the new brush as the background of the Frame
            Background = bgBrush;

            // Set up the transition
            PhoneApplicationPage page = Content as PhoneApplicationPage;
            if (e.NavigationMode == NavigationMode.Back)
            {
                currentTransition = navigationTransitionFactory.CreateTransition(
                    this, NavigationTransitionMode.Backward);
            }
            else
            {
                currentTransition = navigationTransitionFactory.CreateTransition(
                    this, NavigationTransitionMode.Forward);
            }

            currentTransition.Completed += transition_Completed;
            currentTransition.Begin();
        }

        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            // Stop the transition
            clearTransition();

            // Rotating from
            PageOrientation from = previousOrientation;

            // Rotating to
            PageOrientation to = e.Orientation;

            // Cache the new orientation
            previousOrientation = e.Orientation;

            // Which way are we rotating?
            RotationTransitionMode mode = getRotation(from, to);

            if (mode != RotationTransitionMode.None)
            {
                // Set up the transition
                currentTransition = rotationTransitionFactory.CreateTransition(
                    (UIElement)Content, mode);
                currentTransition.Completed += transition_Completed;
                currentTransition.Begin();
            }
        }

        void transition_Completed(object sender, System.EventArgs e)
        {
            // Stop the transition
            clearTransition();

            // Clear the background brush and the cached bitmap
            clearBitmap();
        }

        bool transitionEnabled()
        {
            // Get the content page
            PhoneApplicationPage page = Content as PhoneApplicationPage;

            // Get whether the value of the attached property
            return (bool)page.GetValue(TransitionService.NavigationTransitionEnabledProperty);
        }

        void clearTransition()
        {
            if (currentTransition != null)
            {
                currentTransition.Stop();
                currentTransition = null;
            }
        }

        void clearBitmap()
        {
            // Clear the ImageBrush
            Background = null;

            // Clear the cached bitmap (if any)
            bitmap = null;
        }

        RotationTransitionMode getRotation(PageOrientation from, PageOrientation to)
        {
            RotationTransitionMode mode = RotationTransitionMode.None;

            if (from == PageOrientation.PortraitUp)
            {
                if (to == PageOrientation.LandscapeLeft)
                {
                    mode = RotationTransitionMode.CounterClockwise;
                }
                else if (to == PageOrientation.LandscapeRight)
                {
                    mode = RotationTransitionMode.Clockwise;
                }
            }
            else if (from == PageOrientation.LandscapeLeft)
            {
                if (to == PageOrientation.PortraitUp)
                {
                    mode = RotationTransitionMode.Clockwise;
                }
                else if (to == PageOrientation.LandscapeRight)
                {
                    mode = RotationTransitionMode.UpsideDownCW;
                }
            }
            else if (from == PageOrientation.LandscapeRight)
            {
                if (to == PageOrientation.PortraitUp)
                {
                    mode = RotationTransitionMode.CounterClockwise;
                }
                else if (to == PageOrientation.LandscapeLeft)
                {
                    mode = RotationTransitionMode.UpsideDownCCW;
                }
            }

            return mode;
        }
    }
}
