using Microsoft.Phone.Controls;
using System;
using System.IO.IsolatedStorage;
using SvetlinAnkov.Albite.Tests.Model;
using SvetlinAnkov.Albite.Tests.View;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Container;

namespace SvetlinAnkov.Albite.Tests
{
    public partial class TestPage : PhoneApplicationPage
    {
        private static readonly string tag = "TestPage";

        public TestPage()
        {
            InitializeComponent();
        }

        private void removeStore()
        {
            // Remove the whole iso store for this app
            Log.D(tag, "Clearing Isolated Storage...");
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

        private void WebBrowserStabilityTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Test/WebBrowserStabilityTestPage.xaml", UriKind.Relative));
        }

        private void LibraryTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Remove all local data
            removeStore();

            Book.Descriptor[] books
                = {
                      new Book.Descriptor(
                          "Test/epub/aliceDynamic.epub", BookContainerType.Epub)
                  };
            new LibraryAddBookTest("Test/Library/", books).Test();
        }

        private void ReaderPageTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Remove all local data
            removeStore();

            // Now run the actual test
            new ReaderPageTest("Test/epub/aliceDynamic.epub", NavigationService).Test();
        }

        private void SpeechSynthesisTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new SpeechSynthesisTest().Test();
        }
    }
}