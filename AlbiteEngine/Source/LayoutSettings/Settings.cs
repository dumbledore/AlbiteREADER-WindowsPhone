using System;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class Settings
    {
        public FontSettings FontSettings { get; set; }
        public TextSettings TextSettings { get; set; }
        public MarginSettings MarginSettings { get; set; }

        /// <summary>
        /// Color theme. Default is Theme.DefaultTheme
        /// </summary>
        public Theme Theme { get; set; }

        public Settings()
        {
            FontSettings = new FontSettings();
            TextSettings = new TextSettings();
            MarginSettings = new MarginSettings();
            Theme = Theme.DefaultTheme;
        }
    }
}
