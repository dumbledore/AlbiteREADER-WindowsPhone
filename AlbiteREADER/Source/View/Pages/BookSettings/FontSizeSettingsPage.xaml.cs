using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class FontSizeSettingsPage : PhoneApplicationPage
    {
        public FontSizeSettingsPage()
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
            int fontSize = settings.FontSettings.FontSize.Size;

            foreach (UIElement element in ContentList.Children)
            {
                ThemeControl control = (ThemeControl)element;
                if ((int) control.FontSize == fontSize)
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

            // Update the font size
            FontSettings fontSettings = new FontSettings(
                settings.FontSettings.Family,
                new FontSize(
                    control.Text,
                    (int)control.FontSize));

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