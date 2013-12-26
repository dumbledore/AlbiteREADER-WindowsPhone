using System.Windows.Media;
namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public static class ColorExtensions
    {
        public static string ToHexString(this Color color)
        {
            int intColor = 0;
            intColor |= color.R << 16;
            intColor |= color.G << 8;
            intColor |= color.B << 0;
            return "#" + intColor.ToString("X6");
        }
    }

    public class Theme
    {
        public Color BackgroundColor { get; private set; }
        public Color TextColor { get; private set; }
        public Color AccentColor { get; private set; }

        public Theme(
            Color backgroundColor, Color textColor, Color accentColor)
        {
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            AccentColor = accentColor;
        }

        public static Theme DefaultTheme { get; private set; }

        static Theme()
        {
            Color chocolate = Color.FromArgb(0xFF, 0x63, 0x4F, 0x3B);
            DefaultTheme = new Theme(Colors.White, Colors.Black, chocolate);
        }
    }
}
