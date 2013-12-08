using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.READER.Source.View.Pages
{
    public partial class BookSettingsPage : PhoneApplicationPage
    {
        public BookSettingsPage()
        {
            InitializeComponent();
            initializeAnimation();
        }

        private DoubleAnimation opacityAnimation;
        private DoubleAnimation scaleXAnimation;
        private DoubleAnimation scaleYAnimation;
        private Storyboard storyboard;

        private void initializeAnimation()
        {
            // Set up the render transform for LayoutRoot
            LayoutRoot.RenderTransformOrigin = new Point(0.5, 0.5);

            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;

            LayoutRoot.RenderTransform = scaleTransform;

            // Set up the easing function
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseIn;

            // Set up the opacity animation
            opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            opacityAnimation.EasingFunction = easingFunction;
            Storyboard.SetTarget(opacityAnimation, LayoutRoot);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));

            // Set up the scaleX animation
            scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.From = 1.05;
            scaleXAnimation.To = 1;
            scaleXAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            scaleXAnimation.EasingFunction = easingFunction;
            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            // Set up the scaleY animation
            scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.From = 1.05;
            scaleYAnimation.To = 1;
            scaleYAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            scaleYAnimation.EasingFunction = easingFunction;
            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

            // Set up the storyboard
            storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            storyboard.Begin();
            base.OnNavigatedTo(e);
        }
    }
}