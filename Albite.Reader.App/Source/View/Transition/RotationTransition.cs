using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Albite.Reader.App.View.Transition
{
    public class RotationTransition : AbstractTransition, IRotationTransition
    {
        public RotationTransitionMode Mode { get; private set; }

        public RotationTransition(UIElement element, RotationTransitionMode mode, Duration duration)
        {
            Mode = mode;

            initializeContentAnimation(element, duration);
        }

        private void initializeContentAnimation(UIElement root, Duration duration)
        {
            // Set up the rotation transform
            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = 0;
            root.RenderTransform = rotateTransform;
            root.RenderTransformOrigin = new Point(0.5, 0.5);

            // Set up an easing function
            CircleEase ease = new CircleEase();
            ease.EasingMode = EasingMode.EaseOut;

            // Set up the rotation animation
            DoubleAnimation rotationAnimation = new DoubleAnimation();
            rotationAnimation.From = getRotationFrom();
            rotationAnimation.To = 0;
            rotationAnimation.Duration = duration;
            rotationAnimation.EasingFunction = ease;
            Storyboard.SetTarget(rotationAnimation, rotateTransform);
            Storyboard.SetTargetProperty(rotationAnimation, new PropertyPath(RotateTransform.AngleProperty));

            // Set up the storyboard
            Storyboard.Children.Add(rotationAnimation);
        }

        private double getRotationFrom()
        {
            {
                switch (Mode)
                {
                    case RotationTransitionMode.Clockwise:
                        return 90;

                    case RotationTransitionMode.CounterClockwise:
                        return -90;

                    case RotationTransitionMode.UpsideDownCW:
                        return 180;

                    case RotationTransitionMode.UpsideDownCCW:
                        return -180;

                    default:
                        throw new InvalidOperationException("Invalid mode");
                }
            }
        }

        public class Factory : IRotationTransitionFactory
        {
            public Duration Duration { get; private set; }

            public Factory(Duration duration)
            {
                Duration = duration;
            }

            public IRotationTransition CreateTransition(UIElement element, RotationTransitionMode mode)
            {
                return new RotationTransition(element, mode, Duration);
            }
        }
    }
}
