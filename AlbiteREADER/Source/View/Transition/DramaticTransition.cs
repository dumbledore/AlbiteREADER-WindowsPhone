using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public class DramaticTransition : AbstractTransition, INavigationTransition
    {
        public NavigationTransitionMode Mode { get; private set; }

        public DramaticTransition(ContentControl control, NavigationTransitionMode mode,
            Duration duration, double scaleUp, double scaleDown)
        {
            Mode = mode;

            initializeOldContentAnimation(control.Background, duration, scaleUp, scaleDown);
            initializeNewContentAnimation((UIElement)control.Content, duration, scaleUp, scaleDown);
        }

        private void initializeOldContentAnimation(
            Brush oldContent, Duration duration, double scaleUp, double scaleDown)
        {
            if (oldContent == null)
            {
                // There's nothing to animate
                return;
            }

            // Set up the scale transform
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            scaleTransform.CenterX = 0.5;
            scaleTransform.CenterY = 0.5;
            oldContent.RelativeTransform = scaleTransform;

            // Cache the animation values
            double opacityFrom = getOpacityFrom(true);
            double opacityTo = getOpacityTo(true);
            double scaleFrom = getScaleFrom(true, scaleUp, scaleDown);
            double scaleTo = getScaleTo(true, scaleUp, scaleDown);

            // Set up the opacity animation
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = opacityFrom;
            opacityAnimation.To = opacityTo;
            opacityAnimation.Duration = duration;
            Storyboard.SetTarget(opacityAnimation, oldContent);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Brush.OpacityProperty));

            // Set up the scaleX animation
            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.From = scaleFrom;
            scaleXAnimation.To = scaleTo;
            scaleXAnimation.Duration = duration;
            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            // Set up the scaleY animation
            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.From = scaleFrom;
            scaleYAnimation.To = scaleTo;
            scaleYAnimation.Duration = duration;
            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

            // Set up the storyboard
            Storyboard.Children.Add(opacityAnimation);
            Storyboard.Children.Add(scaleXAnimation);
            Storyboard.Children.Add(scaleYAnimation);
        }

        private void initializeNewContentAnimation(
            UIElement root, Duration duration, double scaleUp, double scaleDown)
        {
            // Set up the render transform
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            root.RenderTransform = scaleTransform;
            root.RenderTransformOrigin = new Point(0.5, 0.5);

            // Cache the animation values
            double opacityFrom = getOpacityFrom(false);
            double opacityTo = getOpacityTo(false);
            double scaleFrom = getScaleFrom(false, scaleUp, scaleDown);
            double scaleTo = getScaleTo(false, scaleUp, scaleDown);

            // Set up the opacity animation
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = opacityFrom;
            opacityAnimation.To = opacityTo;
            opacityAnimation.Duration = duration;
            Storyboard.SetTarget(opacityAnimation, root);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Set up the scaleX animation
            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.From = scaleFrom;
            scaleXAnimation.To = scaleTo;
            scaleXAnimation.Duration = duration;
            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            // Set up the scaleY animation
            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.From = scaleFrom;
            scaleYAnimation.To = scaleTo;
            scaleYAnimation.Duration = duration;
            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

            // Set up the storyboard
            Storyboard.Children.Add(opacityAnimation);
            Storyboard.Children.Add(scaleXAnimation);
            Storyboard.Children.Add(scaleYAnimation);
        }

        private double getOpacityFrom(bool goingAway)
        {
            if (goingAway)
            {
                return 1.0; // Fade out
            }
            else
            {
                return 0.0; // Fade in
            }
        }

        private double getOpacityTo(bool goingAway)
        {
            if (goingAway)
            {
                return 0.0; // Fade out
            }
            else
            {
                return 1.0; // Fade in
            }
        }

        private double getScaleFrom(bool goingAway, double scaleUp, double scaleDown)
        {
            if (goingAway)
            {
                return 1.0; // Going away
            }
            else
            {
                switch (Mode)
                {
                    case NavigationTransitionMode.Backward:
                        return scaleDown; // Coming back from below

                    case NavigationTransitionMode.Forward:
                        return scaleUp; // Coming back from above

                    default:
                        throw new InvalidOperationException("Invalid mode");
                }
            }
        }

        private double getScaleTo(bool goingAway, double scaleUp, double scaleDown)
        {
            if (goingAway)
            {
                switch (Mode)
                {
                    case NavigationTransitionMode.Backward:
                        return scaleUp; // Current page is going out from the stack

                    case NavigationTransitionMode.Forward:
                        return scaleDown; // Current page is going to the back of the stack

                    default:
                        throw new InvalidOperationException("Invalid mode");
                }
            }
            else
            {
                return 1.0; // New page is coming to the top of the stack
            }
        }

        public class Factory : INavigationTransitionFactory
        {
            public Duration Duration { get; private set; }
            public double ScaleUp { get; private set; }
            public double ScaleDown { get; private set; }

            public Factory(Duration duration, double scaleUp, double scaleDown)
            {
                Duration = duration;
                ScaleUp = scaleUp;
                ScaleDown = scaleDown;
            }

            public INavigationTransition CreateTransition(ContentControl control, NavigationTransitionMode mode)
            {
                return new DramaticTransition(control, mode, Duration, ScaleUp, ScaleDown);
            }
        }
    }
}
