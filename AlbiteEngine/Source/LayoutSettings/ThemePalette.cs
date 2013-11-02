using System.Collections.Generic;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.Engine.LayoutSettings
{
    public class ThemePalette
    {
        Dictionary<string, ThemeColor> colors = new Dictionary<string,ThemeColor>();

        public void AddColor(string name, Color color)
        {
            if (colors.ContainsKey(name))
            {
                // Update the currently set color
                colors[name].Color = color;
            }
            else
            {
                // Create a new color
                colors[name] = new ThemeColor(this, name, color);
            }
        }

        public ThemeColor this[string name]
        {
            get { return colors[name]; }
        }
    }
}
