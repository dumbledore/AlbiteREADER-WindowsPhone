using System;
using System.Windows;

namespace SvetlinAnkov.Albite.READER.View.Transition
{
    public static class TransitionService
    {
        public static readonly DependencyProperty NavigationTransitionEnabledProperty =
        DependencyProperty.RegisterAttached(
                "NavigationTransitionEnabled", typeof(bool), typeof(TransitionService), null);

        public static bool GetNavigationTransitionEnabled(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(NavigationTransitionEnabledProperty);
        }

        public static void SetNavigationTransitionEnabled(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(NavigationTransitionEnabledProperty, value);
        }
    }
}
