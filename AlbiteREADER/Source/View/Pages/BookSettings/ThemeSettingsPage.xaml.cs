using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows;
using System.Windows.Input;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class ThemeSettingsPage : PhoneApplicationPage
    {
        public ThemeSettingsPage()
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

            // Get current theme
            Theme theme = settings.Theme;

            foreach (UIElement element in ContentList.Children)
            {
                ThemeControl control = (ThemeControl)element;
                if (control.BackgroundColor == theme.BackgroundColor
                    && control.ForegroundColor == theme.TextColor)
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

            // Update the theme
            Theme theme = new Theme(
                control.Text,
                control.ForegroundColor,
                control.BackgroundColor);

            // Create the new settings
            LayoutSettings newSettings
                = new LayoutSettings(
                    settings.FontSettings,
                    settings.TextSettings,
                    settings.MarginSettings,
                    theme);

            // Update & persist
            context.LayoutSettings = newSettings;

            // Go back
            NavigationService.GoBack();
        }
    }
}