using SvetlinAnkov.Albite.Engine.LayoutSettings;

namespace SvetlinAnkov.Albite.READER.Utils
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

                    settings.LineHeight     = 140; // in %
                    settings.FontSize       = 28; // in px
                    settings.FontFamily     = "Georgia";
                    settings.Justified      = true;
                    settings.MarginTop      = 0.0375f; //in % of the longer side
                    settings.MarginBottom   = 0.0375f; //in % of the longer side
                    settings.MarginLeft     = 0.0375f; //in % of the longer side
                    settings.MarginRight    = 0.0375f; //in % of the longer side
                    settings.Theme          = DefaultTheme;

                    return settings;
                }
            }

            // Default themes
            public static readonly Theme DayTheme
                = new Theme("white", "black", "#634F3B");

            public static readonly Theme NightTheme
                = new Theme("black", "white", "#634F3B");

            public static readonly Theme DefaultTheme = DayTheme;

        }
    }
}
