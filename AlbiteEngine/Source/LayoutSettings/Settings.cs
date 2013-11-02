using System;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class Settings
    {
        public int LineHeight { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public bool Justified { get; set; }

        public float MarginTop { get; set; }
        public float MarginBottom { get; set; }
        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }

        public Theme Theme { get; set; }
    }
}
