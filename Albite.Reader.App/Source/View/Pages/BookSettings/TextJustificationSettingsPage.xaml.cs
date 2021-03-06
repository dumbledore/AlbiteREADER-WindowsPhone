﻿using Microsoft.Phone.Controls;
using Albite.Reader.Engine.Layout;
using System.Windows.Navigation;

namespace Albite.Reader.App.View.Pages.BookSettings
{
    public partial class TextJustificationSettingsPage : PhoneApplicationPage
    {
        public TextJustificationSettingsPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Get current layout settings
            LayoutSettings settings = App.Context.LayoutSettings;

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
            // Get current layout settings
            LayoutSettings settings = App.Context.LayoutSettings;

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
            App.Context.LayoutSettings = newSettings;
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