using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Albite.Reader.Speech.Narration;

namespace Albite.Reader.App.View.Pages.Narration
{
    public partial class NarrationSpeedSettingsPage : PhoneApplicationPage
    {
        public NarrationSpeedSettingsPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Get the current settings
            NarrationSettings settings = App.Context.NarrationSettings;

            // Update the slider
            SpeedSlider.Value = settings.BaseSpeedRatio;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        private void applySpeed()
        {
            // Get the current settings
            NarrationSettings settings = App.Context.NarrationSettings;

            // Update speed in settings
            settings.BaseSpeedRatio = (float)Math.Round(SpeedSlider.Value, 1);

            // Update & persist
            App.Context.NarrationSettings = settings;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // First apply the new settings
            applySpeed();

            // Go on as usual
            base.OnNavigatingFrom(e);
        }
    }
}