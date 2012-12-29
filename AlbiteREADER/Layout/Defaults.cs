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

namespace SvetlinAnkov.AlbiteREADER.Layout
{
    public class Defaults
    {
        // Default themes
        public static readonly Theme DayTheme
            = new Theme("white", "black", "#634F3B");

        public static readonly Theme NightTheme
            = new Theme("black", "white", "#634F3B");

        public static readonly Theme DefaultTheme = DayTheme;

        // Default settings
        public static Settings DefaultSettings
        {
            get
            {
                Settings settings = new Settings();

                settings.LineHeight     = 140;  // in %
                settings.FontSize       = 28;   // in px
                settings.FontFamily     = "Georgia";
                settings.TextAlign      = TextAlign.Justify;
                settings.MarginTop      = 30;
                settings.MarginBottom   = 30;
                settings.MarginLeft     = 30;
                settings.MarginRight    = 40;
                settings.Theme          = DefaultTheme;

                return settings;
            }
        }
    }
}
