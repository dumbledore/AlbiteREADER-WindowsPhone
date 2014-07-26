using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary.Test;
using Albite.Reader.Core.Diagnostics;
using System;
using System.IO.IsolatedStorage;
using Albite.Reader.Speech.Test;
using Albite.Reader.Core.Test;

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
            using (TestCase test = new Tests())
            {
                test.Test();
            }
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

            using (TestCase test = new LibraryAddBookTest("Test/Library/", books))
            {
                test.Test();
            }
        }

        private void ReaderPageTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Remove all local data
            removeStore();

            // Now run the actual test
            using (TestCase test = new ReaderPageTest("Test/epub/aliceDynamic.epub", NavigationService))
            {
                test.Test();
            }
        }

        private void SpeechSynthesisTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (TestCase test = new XhtmlNarratorTest("Test/Speech/down-the-rabbit-hole.xhtml"))
            {
                test.Test();
            }
        }

        private void PaginationTestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (TestCase test = new PaginationTest("Test/epub/aliceDynamic.epub", 3, 16, NavigationService))
            {
                test.Test();
            }
        }
    }
}