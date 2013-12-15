using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public class TransitionFrame : PhoneApplicationFrame
    {
        private ITransitionFactory transitionFactory;
        private ITransition currentTransition;
        private WriteableBitmap bitmap;

        public TransitionFrame(ITransitionFactory transitionFactory)
        {
            this.transitionFactory = transitionFactory;

            Navigating += OnNavigating;
            Navigated += OnNavigated;
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

            // Cache the current screen
            bitmap = new WriteableBitmap(Content as UIElement, null);
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Stop any previous transitions
            clearTransition();

            // Do we want the transition?
            if (!e.IsNavigationInitiator || !transitionEnabled())
            {
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
                currentTransition = transitionFactory.CreateTransition(
                    this, ITransitionMode.Backward);
            }
            else
            {
                currentTransition = transitionFactory.CreateTransition(
                    this, ITransitionMode.Forward);
            }

            currentTransition.Completed += transition_Completed;
            currentTransition.Begin();
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
    }
}
