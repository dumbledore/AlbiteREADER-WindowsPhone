using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SvetlinAnkov.Albite.READER.BrowserEngine;
using SvetlinAnkov.Albite.READER.Resources.Defaults;

namespace SvetlinAnkov.Albite.READER.Utils
{
    public static class Defaults
    {
        public static class Application
        {
            // Non so far
        }

        public static class Layout
        {
            // Default settings
            public static Settings DefaultSettings
            {
                get
                {
                    Settings settings = new Settings();

                    settings.LineHeight = int.Parse(LayoutDefaults.LineHeight);
                    settings.FontSize = int.Parse(LayoutDefaults.FontSize);
                    settings.FontFamily = LayoutDefaults.FontFamily;
                    settings.TextAlign = TextAlign.FromString(LayoutDefaults.TextAlign);
                    settings.MarginTop = int.Parse(LayoutDefaults.MarginTop);
                    settings.MarginBottom = int.Parse(LayoutDefaults.MarginBottom);
                    settings.MarginLeft = int.Parse(LayoutDefaults.MarginLeft);
                    settings.MarginRight = int.Parse(LayoutDefaults.MarginRight);
                    settings.Theme = DefaultTheme;

                    return settings;
                }
            }

            // Default themes
            public static readonly Theme DayTheme
                = new Theme(
                    LayoutDefaults.DayThemeBackroundColor,
                    LayoutDefaults.DayThemeTextColor,
                    LayoutDefaults.DayThemeAccentColor);

            public static readonly Theme NightTheme
                = new Theme(
                    LayoutDefaults.NightThemeBackgroundColor,
                    LayoutDefaults.NightThemeTextColor,
                    LayoutDefaults.NightThemeAccentColor);

            public static readonly Theme DefaultTheme = DayTheme;

        }
    }
}
