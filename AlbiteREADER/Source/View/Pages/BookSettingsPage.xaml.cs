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

        private void ThemeControl_Tap(object sender, GEArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/ThemeSettingsPage.xaml", UriKind.Relative));
        }
    }
}