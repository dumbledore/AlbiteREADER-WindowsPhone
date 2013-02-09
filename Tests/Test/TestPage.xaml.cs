using Microsoft.Phone.Controls;
using System;

namespace SvetlinAnkov.Albite.Tests.Test
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void AutomatedTestsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Tests().Test();
        }

        private void WebBrowserSecurityTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/WebBrowserSecurityTestPage.xaml", UriKind.Relative));
        }
    }
}