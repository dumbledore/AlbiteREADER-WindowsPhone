using System.Runtime.Serialization;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.Engine.Layout
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
        /// <summary>
        /// Name for this theme,
        /// e.g. black on white, etc.
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public Color TextColor { get; private set; }

        [DataMember]
        public Color BackgroundColor { get; private set; }

        [DataMember]
        public Color AccentColor { get; private set; }

        public Theme(
            string name,
            Color textColor, Color backgroundColor, Color accentColor)
        {
            Name = name;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            AccentColor = accentColor;
        }

        public Theme(string name, Color textColor, Color backgroundColor)
            : this(name, textColor, backgroundColor, textColor) { }
    }
}
