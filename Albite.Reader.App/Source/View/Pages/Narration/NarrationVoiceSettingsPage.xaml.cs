using Microsoft.Phone.Controls;
using Albite.Reader.App.View.Controls;
using System.Windows.Navigation;
using Albite.Reader.Speech.Narration;
using System.Collections.ObjectModel;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages.Narration
{
    public partial class NarrationVoiceSettingsPage : PhoneApplicationPage
    {
        public NarrationVoiceSettingsPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Go over all voices
            ObservableCollection<NarrationVoice> voices =
                new ObservableCollection<NarrationVoice>(NarrationVoice.Voices);

            VoicesList.ItemsSource = voices;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        private void applySettings(VoiceControl selected)
        {
            if (selected != null)
            {
                // Get current settings
                NarrationSettings settings = App.Context.NarrationSettings;

                // Update voice
                settings.BaseVoice = selected.Voice;

                // Update & persist
                App.Context.NarrationSettings = settings;
            }
        }

        private void VoiceHeaderControl_Tap(object sender, GEArgs e)
        {
            // First apply the new settings
            applySettings((VoiceControl)sender);

            // Go back
            NavigationService.GoBack();
        }
    }
}