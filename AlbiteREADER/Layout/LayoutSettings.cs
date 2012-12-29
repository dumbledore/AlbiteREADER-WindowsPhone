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
    public class LayoutSettings
    {
        public int LineHeight { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public TextAlign TextAlign { get; set; }

        public int MarginTop { get; set; }
        public int MarginBottom { get; set; }
        public int MarginLeft { get; set; }
        public int MarginRight { get; set; }

        public LayoutTheme Theme { get; set; }
    }

    public class TextAlign
    {
        private string text;

        private TextAlign(string text)
        {
            this.text = text;
        }

        public override string ToString()
        {
            return text;
        }

        public static readonly TextAlign Left = new TextAlign("left");
        public static readonly TextAlign Right = new TextAlign("right");
        public static readonly TextAlign Center = new TextAlign("center");
        public static readonly TextAlign Justify = new TextAlign("justify");
    }
}
