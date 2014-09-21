using Albite.Reader.Speech.Narration;
using Microsoft.Phone.Controls;
using System;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages.Narration
{
    public partial class NarrationSettingsPage : PhoneApplicationPage
    {
        public NarrationSettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Update buttons
            updateStatusValues();

            base.OnNavigatedTo(e);
        }

        private void updateStatusValues()
        {
            // Get current narration settings
            NarrationSettings settings = App.Context.NarrationSettings;

            // Set current speed
            SpeedControl.ContentText = settings.BaseSpeedRatio.ToString();

            // Set current voice
            VoiceControl.ContentText = settings.BaseVoice.Name;
        }

        private void Speed_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/Narration/NarrationSpeedSettingsPage.xaml", UriKind.Relative));
        }

        private void Voice_Tap(object sender, GEArgs e)
        {
        }
    }
}