using SvetlinAnkov.Albite.Engine.Layout;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public static class DefaultLayoutSettings
    {
        public static readonly LayoutSettings LayoutSettings
            = new LayoutSettings(
                // Font family & size
                new FontSettings(
                    "Georgia",
                    new FontSize(
                        "medium", 28)),

                // Line height & justification
                new TextSettings(
                    new LineHeight(
                        "medium", 140),
                        true), // justified

                // Margins
                new MarginSettings(
                    "medium", 4),

                // Colours theme
                new Theme(
                    "black on white", Colors.Black, Colors.White));
    }
}
