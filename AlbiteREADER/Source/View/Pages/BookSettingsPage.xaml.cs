using Microsoft.Phone.Controls;
using System;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class BookSettingsPage : PhoneApplicationPage
    {
        public BookSettingsPage()
        {
            InitializeComponent();
        }

        private void Theme_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/ThemeSettingsPage.xaml", UriKind.Relative));
        }

        private void FontFamily_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/FontFamilySettingsPage.xaml", UriKind.Relative));
        }

        private void FontSize_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/FontSizeSettingsPage.xaml", UriKind.Relative));
        }

        private void TextJustification_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/TextJustificationSettingsPage.xaml", UriKind.Relative));
        }

        private void LineSpacing_Tap(object sender, GEArgs e)
        {
        }

        private void PageMargins_Tap(object sender, GEArgs e)
        {
        }
    }
}