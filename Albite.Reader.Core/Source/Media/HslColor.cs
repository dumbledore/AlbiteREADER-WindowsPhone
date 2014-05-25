using System;
using System.Windows.Media;

namespace Albite.Reader.Core.Media
{
    public class HslColor
    {
        /// <summary>
        /// Color hue. Must be within [0, 360]
        /// </summary>
        public double Hue { get; private set; }

        /// <summary>
        /// Color saturation. Must be within [0, 1]
        /// </summary>
        public double Saturation { get; private set; }

        /// <summary>
        /// Color lightness. Must be within [0, 1]
        /// </summary>
        public double Lightness { get; private set; }

        public HslColor(double hue, double saturation, double lightness)
        {
            setValues(hue, saturation, lightness);
        }

        private void setValues(double h, double s, double l)
        {
            Hue = h < 0 ? 0 : h > 360 ? 360 : h;
            Saturation = s < 0 ? 0 : s > 1 ? 1 : s;
            Lightness = l < 0 ? 0 : l > 1 ? 1 : l;
        }

        public Color ToColor()
        {
            return hslToColor(Hue, Saturation, Lightness);
        }

        private static Color hslToColor(double h, double s, double l)
        {
            // Clamp hsl
            h = h < 0 ? 0 : h > 360 ? 360 : h;
            s = s < 0 ? 0 : s > 1 ? 1 : s;
            l = l < 0 ? 0 : l > 1 ? 1 : l;

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double h2 = h / 60f;
            double x = c * (1 - Math.Abs((h2 % 2) - 1));

            double r1, g1, b1;

            if (h2 < 1) // 0 <= h2 < 1
            {
                r1 = c;
                g1 = x;
                b1 = 0;
            }
            else if (h2 < 2) // 1 <= h2 < 2
            {
                r1 = x;
                g1 = c;
                b1 = 0;
            }
            else if (h2 < 3) // 2 <= h2 < 3
            {
                r1 = 0;
                g1 = c;
                b1 = x;
            }
            else if (h2 < 4) // 3 <= h2 < 4
            {
                r1 = 0;
                g1 = x;
                b1 = c;
            }
            else if (h2 < 5) // 4 <= h2 < 5
            {
                r1 = x;
                g1 = 0;
                b1 = c;
            }
            else // 5 <= h2 < 6
            {
                r1 = c;
                g1 = 0;
                b1 = x;
            }

            double m = l - 0.5 * c;
            double r = (r1 + m) * 255;
            double g = (g1 + m) * 255;
            double b = (b1 + m) * 255;

            return Color.FromArgb(0xFF, (byte)r, (byte)g, (byte)b);
        }
    }
}
