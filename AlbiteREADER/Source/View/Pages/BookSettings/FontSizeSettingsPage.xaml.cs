using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class FontSizeSettingsPage : PhoneApplicationPage
    {
        public FontSizeSettingsPage()
        {
            InitializeComponent();
        }

        private ThemeControl selected = null;

        private void setCurrentState()
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
                if ((int)control.FontSize == fontSize)
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
                // Get the context
                AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

                // Get current layout settings
                LayoutSettings settings = context.LayoutSettings;

                // Update the font size
                FontSettings fontSettings = new FontSettings(
                    settings.FontSettings.Family,
                    new FontSize(
                        selected.Text,
                        (int)selected.FontSize));

                // Create the new settings
                LayoutSettings newSettings
                    = new LayoutSettings(
                        fontSettings,
                        settings.TextSettings,
                        settings.MarginSettings,
                        settings.Theme);

                // Update & persist
                context.LayoutSettings = newSettings;
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