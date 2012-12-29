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
    public class LayoutTheme
    {
        private readonly int backgroundColor;
        private readonly int fontColor;
        private readonly int accentColor;

        public LayoutTheme(
            int backgroundColor, int fontColor, int accentColor)
        {
            this.backgroundColor = backgroundColor;
            this.fontColor = fontColor;
            this.accentColor = accentColor;
        }

        public int BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
        }

        public int FontColor
        {
            get
            {
                return fontColor;
            }
        }

        public int AccentColor
        {
            get
            {
                return accentColor;
            }
        }

        // Default themes
    }
}
