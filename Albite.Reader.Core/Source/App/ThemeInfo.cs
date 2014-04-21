using System.Windows;

namespace Albite.Reader.Core.App
{
    public static class ThemeInfo
    {
        public static bool ThemeIsDark
        {
            get
            {
                Visibility darkVisibility
                    = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];

                return darkVisibility == Visibility.Visible;
            }
        }
    }
}
