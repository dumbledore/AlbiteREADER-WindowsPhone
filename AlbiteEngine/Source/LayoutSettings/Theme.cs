using System.Windows.Media;
namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class Theme
    {
        public ThemeColor BackgroundColor { get; private set; }
        public ThemeColor TextColor { get; private set; }
        public ThemeColor AccentColor { get; private set; }

        public Theme(
            ThemeColor backgroundColor, ThemeColor textColor, ThemeColor accentColor)
        {
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            AccentColor = accentColor;
        }

        public static Theme DefaultTheme { get; private set; }

        static Theme()
        {
            ThemePalette palette = new ThemePalette();
            palette.AddColor("White", Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            palette.AddColor("Black", Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
            palette.AddColor("Chocolate", Color.FromArgb(0xFF, 0x63, 0x4F, 0x3B));
            DefaultTheme = new Theme(
                palette["White"], palette["Black"], palette["Chocolate"]);
        }
    }
}
