﻿using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.READER.View.Controls;
using System.Windows.Input;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.Settings
{
    public partial class ThemeSettingsPage : PhoneApplicationPage
    {
        public ThemeSettingsPage()
        {
            InitializeComponent();
        }

        private void ThemeControl_Tap(object sender, GEArgs e)
        {
            ThemeControl control = (ThemeControl)sender;

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            SvetlinAnkov.Albite.Engine.LayoutSettings.Settings settings = context.Settings;

            // Update the theme
            settings.Theme = new SvetlinAnkov.Albite.Engine.LayoutSettings.Theme(
                control.ForegroundColor,
                control.BackgroundColor);

            // Update & persist
            context.Settings = settings;

            // Go back
            NavigationService.GoBack();
        }
    }
}