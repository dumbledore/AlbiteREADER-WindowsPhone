using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Engine.Layout;
using System;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages.BookSettings
{
    public partial class BookSettingsPage : PhoneApplicationPage
    {
        public BookSettingsPage()
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
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            LayoutSettings settings = context.LayoutSettings;

            // Set current theme name
            ThemeControl.ContentText = settings.Theme.Name;

            // Set current font family name
            FontFamilyControl.ContentText = settings.FontSettings.Family;

            // Set current font size name
            FontSizeControl.ContentText = settings.FontSettings.FontSize.Name;

            // Set current text justification state
            TextJustificationControl.ContentText
                = settings.TextSettings.Justified ? "justified" : "left-aligned";

            // Set current line-spacing text
            LineSpacingControl.ContentText = settings.TextSettings.LineHeight.Name;

            // Set current page margins text
            PageMarginsControl.ContentText = settings.MarginSettings.Name;
        }

        private void Theme_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/BookSettings/ThemeSettingsPage.xaml", UriKind.Relative));
        }

        private void FontFamily_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/BookSettings/FontFamilySettingsPage.xaml", UriKind.Relative));
        }

        private void FontSize_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/BookSettings/FontSizeSettingsPage.xaml", UriKind.Relative));
        }

        private void TextJustification_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/BookSettings/TextJustificationSettingsPage.xaml", UriKind.Relative));
        }

        private void LineSpacing_Tap(object sender, GEArgs e)
        {
        }

        private void PageMargins_Tap(object sender, GEArgs e)
        {
        }
    }
}