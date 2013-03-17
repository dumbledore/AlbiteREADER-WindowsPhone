using Microsoft.Phone.Controls;
using System;
using System.IO.IsolatedStorage;
using SvetlinAnkov.Albite.Tests.Test.Model;

namespace SvetlinAnkov.Albite.Tests.Test
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void removeStore()
        {
            // Remove the whole iso store for this app
            IsolatedStorageFile.GetUserStoreForApplication().Remove();
        }

        private void AutomatedTestsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Remove all local data
            removeStore();

            // Now run the tests
            new Tests().Test();
        }

        private void WebBrowserSecurityTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/WebBrowserSecurityTestPage.xaml", UriKind.Relative));
        }

        private void LibraryTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Remove all local data
            removeStore();

            string[] books
                = {
                      "Test/epub/aliceDynamic.epub"
                  };
            new LibraryAddBookTest("Test/Library/", books).Test();
        }

        private void ReaderPageTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/View/ReaderPage.xaml?id=1", UriKind.Relative));
        }
    }
}