using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
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
            Settings settings = context.Settings;

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
            Settings settings = context.Settings;

            // Update the theme
            settings.Theme = new Theme(
                control.ForegroundColor,
                control.BackgroundColor);

            // Update & persist
            context.Settings = settings;

            // Go back
            NavigationService.GoBack();
        }
    }
}