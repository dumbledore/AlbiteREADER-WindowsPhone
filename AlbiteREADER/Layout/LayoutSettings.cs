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
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public LayoutTheme Theme { get; set; }
    }

    public class LayoutTheme
    {
        public int BackgroundColor { get; set; }
        public int FontColor { get; set; }
    }
}
