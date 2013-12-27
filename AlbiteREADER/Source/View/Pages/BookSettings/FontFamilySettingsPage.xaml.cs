using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
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
            Settings settings = context.Settings;

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
            Settings settings = context.Settings;

            // Update the theme
            settings.FontSettings.Family = control.FontFamily.Source;

            // Update & persist
            context.Settings = settings;

            // Go back
            NavigationService.GoBack();
        }
    }
}