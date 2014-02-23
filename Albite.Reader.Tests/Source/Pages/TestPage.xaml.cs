using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary.Test;
using Albite.Reader.Core.Diagnostics;
using System;
using System.IO.IsolatedStorage;

namespace Albite.Reader.Tests.Pages
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
            NavigationService.Navigate(new Uri("/Source/Pages/WebBrowserSecurityTestPage.xaml", UriKind.Relative));
        }

        private void WebBrowserStabilityTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Source/Pages/WebBrowserStabilityTestPage.xaml", UriKind.Relative));
        }

        private void LibraryTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Remove all local data
            removeStore();

            string[] books
                = new string[] {
                      "Test/epub/aliceDynamic.epub",
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

        private void PaginationTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new PaginationTest("Test/epub/aliceDynamic.epub", 3, 16, NavigationService).Test();
        }
    }
}