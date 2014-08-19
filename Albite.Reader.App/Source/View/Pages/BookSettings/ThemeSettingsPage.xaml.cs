using Microsoft.Phone.Controls;
using Albite.Reader.Engine.Layout;
using Albite.Reader.App.View.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages.BookSettings
{
    public partial class ThemeSettingsPage : PhoneApplicationPage
    {
        public ThemeSettingsPage()
        {
            InitializeComponent();
        }

        private ThemeControl selected = null;

        private void setCurrentState()
        {
            // Get current layout settings
            LayoutSettings settings = App.Context.LayoutSettings;

            // Get current theme
            Theme theme = settings.Theme;

            foreach (UIElement element in ContentList.Children)
            {
                ThemeControl control = (ThemeControl)element;
                if (control.BackgroundColor == theme.BackgroundColor
                    && control.ForegroundColor == theme.TextColor)
                {
                    selected = control;
                    selected.Selected = true;
                    break;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        private void applySettings()
        {
            if (selected != null)
            {
                // Get current layout settings
                LayoutSettings settings = App.Context.LayoutSettings;

                // Update the theme
                Theme theme = new Theme(
                    selected.Text,
                    selected.ForegroundColor,
                    selected.BackgroundColor);

                // Create the new settings
                LayoutSettings newSettings
                    = new LayoutSettings(
                        settings.FontSettings,
                        settings.TextSettings,
                        settings.MarginSettings,
                        theme);

                // Update & persist
                App.Context.LayoutSettings = newSettings;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // First apply the new settings
            applySettings();

            // Go on as usual
            base.OnNavigatingFrom(e);
        }

        private void ThemeControl_Tap(object sender, GEArgs e)
        {
            if (selected != null)
            {
                // Unselect previously selected item
                selected.Selected = false;
            }

            // Get new item to be selected
            selected = (ThemeControl)sender;

            // Select new item
            selected.Selected = true;
        }
    }
}