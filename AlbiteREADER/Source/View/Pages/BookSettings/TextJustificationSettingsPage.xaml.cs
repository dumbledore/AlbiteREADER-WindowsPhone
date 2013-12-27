using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class TextJustificationSettingsPage : PhoneApplicationPage
    {
        public TextJustificationSettingsPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            LayoutSettings settings = context.LayoutSettings;

            // Set justification state
            JustificationSwitch.IsChecked = settings.TextSettings.Justified;
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
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            LayoutSettings settings = context.LayoutSettings;

            // Update the justification state
            bool? justified = JustificationSwitch.IsChecked;

            TextSettings textSettings = new TextSettings(
                settings.TextSettings.LineHeight,
                justified.HasValue ? justified.Value : false);

            // Create the new settings
            LayoutSettings newSettings
                = new LayoutSettings(
                    settings.FontSettings,
                    textSettings,
                    settings.MarginSettings,
                    settings.Theme);

            // Update & persist
            context.LayoutSettings = newSettings;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // First apply the new settings
            applySettings();

            // Go on as usual
            base.OnNavigatingFrom(e);
        }
    }
}