using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class FontFamilySettingsPage : PhoneApplicationPage
    {
        public FontFamilySettingsPage()
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
            string fontFamily = settings.FontSettings.Family;

            foreach (UIElement element in ContentList.Children)
            {
                ThemeControl control = (ThemeControl)element;
                if (fontFamily.Equals(control.FontFamily.Source, System.StringComparison.InvariantCultureIgnoreCase))
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

                // Update the font family
                FontSettings fontSettings = new FontSettings(
                    selected.FontFamily.Source,
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