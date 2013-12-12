using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public class DramaticTransition : ITransition
    {
        public ITransitionMode Mode { get; private set; }
        public event EventHandler Completed;

        public DramaticTransition(UIElement root, ITransitionMode mode,
            Duration duration, double scaleUp, double scaleDown)
        {
            Mode = mode;
            initializeAnimation(root, duration, scaleUp, scaleDown);
        }

        private DoubleAnimation opacityAnimation;
        private DoubleAnimation scaleXAnimation;
        private DoubleAnimation scaleYAnimation;
        private Storyboard storyboard;

        private bool stopRequested = false;

        public void Begin()
        {
            stopRequested = false;
            storyboard.Begin();
        }

        public void Stop()
        {
            stopRequested = true;
            storyboard.SkipToFill();
        }

        void storyboard_Completed(object sender, EventArgs e)
        {
            if (stopRequested)
            {
                return;
            }

            if (Completed != null)
            {
                Completed(this, EventArgs.Empty);
            }
        }

        private void initializeAnimation(
            UIElement layoutRoot, Duration duration, double scaleUp, double scaleDown)
        {
            // Set up the render transform for LayoutRoot
            layoutRoot.RenderTransformOrigin = new Point(0.5, 0.5);

            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;

            layoutRoot.RenderTransform = scaleTransform;

            // Set up the easing function
            //SineEase easingFunction = new SineEase();
            PowerEase easingFunction = new PowerEase();
            easingFunction.Power = 6;
            easingFunction.EasingMode = EasingMode.EaseIn;

            // Set up the opacity animation
            opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = getOpacityFrom();
            opacityAnimation.To = getOpacityTo();
            opacityAnimation.Duration = duration;
            opacityAnimation.EasingFunction = easingFunction;
            Storyboard.SetTarget(opacityAnimation, layoutRoot);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));

            // Set up the scaleX animation
            scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.From = getScaleFrom(scaleUp, scaleDown);
            scaleXAnimation.To = getScaleTo(scaleUp, scaleDown);
            scaleXAnimation.Duration = duration;
            scaleXAnimation.EasingFunction = easingFunction;
            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            // Set up the scaleY animation
            scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.From = getScaleFrom(scaleUp, scaleDown);
            scaleYAnimation.To = getScaleTo(scaleUp, scaleDown);
            scaleYAnimation.Duration = duration;
            scaleYAnimation.EasingFunction = easingFunction;
            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

            // Set up the storyboard
            storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Completed += storyboard_Completed;
        }

        private double getOpacityFrom()
        {
            switch (Mode)
            {
                case ITransitionMode.NavigatingBack:
                case ITransitionMode.NavigatingForward:
                    return 1.0; // Fade out

                case ITransitionMode.NavigatedBack:
                case ITransitionMode.NavigatedForward:
                    return 0.0; // Fade in

                default:
                    throw new InvalidOperationException("Invalid mode");
            }
        }

        private double getOpacityTo()
        {
            switch (Mode)
            {
                case ITransitionMode.NavigatingBack:
                case ITransitionMode.NavigatingForward:
                    return 0.0; // Fade out

                case ITransitionMode.NavigatedBack:
                case ITransitionMode.NavigatedForward:
                    return 1.0; // Fade in

                default:
                    throw new InvalidOperationException("Invalid mode");
            }
        }

        private double getScaleFrom(double scaleUp, double scaleDown)
        {
            switch (Mode)
            {
                case ITransitionMode.NavigatingBack:
                case ITransitionMode.NavigatingForward:
                    return 1.0; // Going away

                case ITransitionMode.NavigatedBack:
                    return scaleDown; // Coming back from below
                case ITransitionMode.NavigatedForward:
                    return scaleUp; // Coming back from above

                default:
                    throw new InvalidOperationException("Invalid mode");
            }
        }

        private double getScaleTo(double scaleUp, double scaleDown)
        {
            switch (Mode)
            {
                case ITransitionMode.NavigatingBack:
                    return scaleUp; // Current page is going out from the stack

                case ITransitionMode.NavigatingForward:
                    return scaleDown; // Current page is going to the back of the stack

                case ITransitionMode.NavigatedBack:
                case ITransitionMode.NavigatedForward:
                    return 1.0; // New page is coming to the top of the stack

                default:
                    throw new InvalidOperationException("Invalid mode");
            }
        }

        public class Factory : ITransitionFactory
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

            public ITransition CreateTransition(UIElement root, ITransitionMode mode)
            {
                return new DramaticTransition(root, mode, Duration, ScaleUp, ScaleDown);
            }
        }
    }
}
