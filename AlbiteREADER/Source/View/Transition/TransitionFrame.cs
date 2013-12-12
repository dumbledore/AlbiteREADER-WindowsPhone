using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public class TransitionFrame : PhoneApplicationFrame
    {
        private ITransitionFactory transitionFactory;
        ITransition currentTransition;

        public TransitionFrame(ITransitionFactory transitionFactory)
        {
            this.transitionFactory = transitionFactory;

            Navigating += OnNavigating;
            Navigated += OnNavigated;
        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            // If the current application is not the origin
            // and destination of the navigation, ignore it.
            // e.g. do not play a transition when the
            // application gets deactivated because the shell
            // will animate the frame out automatically.
            if (!e.IsNavigationInitiator
                || !e.IsCancelable
                || e.NavigationMode != NavigationMode.Back)
            {
                stopTransition();
                return;
            }

            if (!stopTransition())
            {
                e.Cancel = true;
                UIElement content = Content as UIElement;
                currentTransition = transitionFactory.CreateTransition(
                    content, ITransitionMode.NavigatingBack);
                currentTransition.Completed += backTransition_Completed;
                currentTransition.Begin();
            }
        }

        /// <summary>
        /// Stops and clears the backward transition (if any)
        /// </summary>
        /// <returns>True if there was an active transition</returns>
        bool stopTransition()
        {
            if (currentTransition != null)
            {
                currentTransition.Stop();
                currentTransition = null;
                return true;
            }

            return false;
        }

        void backTransition_Completed(object sender, System.EventArgs e)
        {
            GoBack();
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            stopTransition();

            //if (!e.IsNavigationInitiator)
            //{
            //    return;
            //}

            UIElement content = Content as UIElement;
            if (e.NavigationMode == NavigationMode.Back)
            {
                currentTransition = transitionFactory.CreateTransition(
                    content, ITransitionMode.NavigatedBack);
            }
            else
            {
                currentTransition = transitionFactory.CreateTransition(
                    content, ITransitionMode.NavigatedForward);
            }
            currentTransition.Completed += forwardTransition_Completed;
            currentTransition.Begin();
        }

        void forwardTransition_Completed(object sender, System.EventArgs e)
        {
            currentTransition.Stop();
            currentTransition = null;
        }
    }
}
