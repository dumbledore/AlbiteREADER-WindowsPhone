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
using SvetlinAnkov.AlbiteREADER.Layout;
using SvetlinAnkov.AlbiteREADER.Resources.Defaults;

namespace SvetlinAnkov.AlbiteREADER.Utils
{
    public static class Defaults
    {
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

        public static class Application
        {
            public static string Resources { get { return ApplicationDefaults.Resources; } }
        }

        public static class Engine
        {
            public static string LayoutPath { get { return EngineDefaults.LayoutPath; } }
            public static string BookPath { get { return EngineDefaults.BookPath; } }

            public static string JSEngine { get { return EngineDefaults.JSEngine; } }
            public static string MainPage { get { return EngineDefaults.MainPage; } }
            public static string BaseStyles { get { return EngineDefaults.BaseStyles; } }
            public static string ContentStyles { get { return EngineDefaults.ContentStyles; } }
            public static string ThemeStyles { get { return EngineDefaults.ThemeStyles; } }
        }

        public static class Test
        {
            public static string Path { get { return TestDefaults.Path; } }
        }
    }
}
