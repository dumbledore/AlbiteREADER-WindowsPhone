using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class FontFamilySettingsPage : PhoneApplicationPage
    {
        public FontFamilySettingsPage()
        {
            InitializeComponent();
            setDefaultItem();
        }

        private void setDefaultItem()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            LayoutSettings settings = context.LayoutSettings;

            // Get current font family
            string fontFamily = settings.FontSettings.Family;

            foreach (UIElement element in ContentList.Children)
            {
                ThemeControl control = (ThemeControl)element;
                if (fontFamily.Equals(control.FontFamily.Source, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    control.Selected = true;
                    break;
                }
            }
        }

        private void ThemeControl_Tap(object sender, GEArgs e)
        {
            ThemeControl control = (ThemeControl)sender;

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            LayoutSettings settings = context.LayoutSettings;

            // Update the font family
            FontSettings fontSettings = new FontSettings(
                control.FontFamily.Source,
                settings.FontSettings.FontSize);

            // Create the new settings
            LayoutSettings newSettings
                = new LayoutSettings(
                    fontSettings,
                    settings.TextSettings,
                    settings.MarginSettings,
                    settings.Theme);

            // Update & persist
            context.LayoutSettings = newSettings;

            // Go back
            NavigationService.GoBack();
        }
    }
}