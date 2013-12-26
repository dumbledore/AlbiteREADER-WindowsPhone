using System.Runtime.Serialization;
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

    [DataContract]
    public class Theme
    {
        [DataMember]
        public Color TextColor { get; private set; }

        [DataMember]
        public Color BackgroundColor { get; private set; }

        [DataMember]
        public Color AccentColor { get; private set; }

        public Theme(
            Color textColor, Color backgroundColor, Color accentColor)
        {
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            AccentColor = accentColor;
        }

        public Theme(Color textColor, Color backgroundColor)
            : this(textColor, backgroundColor, textColor) { }

        public static Theme DefaultTheme { get; private set; }

        static Theme()
        {
            DefaultTheme = new Theme(Colors.Black, Colors.White);
        }
    }
}
