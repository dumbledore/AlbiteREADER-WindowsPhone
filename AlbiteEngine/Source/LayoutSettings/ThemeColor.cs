using System.Windows.Media;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class ThemeColor
    {
        public string Name { get; private set; }
        public ThemePalette Palette { get; private set; }
        public Color Color { get; internal set; }

        internal ThemeColor(ThemePalette palette, string name, Color color)
        {
            Palette = palette;
            Color = color;
            Name = name;
        }

        public string HtmlColor
        {
            get
            {
                int color = 0;
                color |= Color.R << 16;
                color |= Color.G << 8;
                color |= Color.B << 0;
                return "#" + color.ToString("X6");
            }
        }
    }
}
