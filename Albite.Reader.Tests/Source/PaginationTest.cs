using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Test;
using Albite.Reader.Container;
using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;
using System;
using System.Windows.Navigation;

namespace Albite.Reader.Tests
{
    public class PaginationTest : TestCase
    {
        public static readonly string TestFolder = "Test/Pagination/Library";

        private string book;
        private int chapterNumber;
        private int expectedPageCount;
        private NavigationService navigation;

        public PaginationTest(
            string book,
            int chapterNumber,
            int expectedPageCount,
            NavigationService navigation)
        {
            this.book = book;
            this.chapterNumber = chapterNumber;
            this.expectedPageCount = expectedPageCount;
            this.navigation = navigation;
        }

        protected override void TestImplementation()
        {
            Log("Removing previous data");
            using (IsolatedStorage a = new IsolatedStorage(TestFolder))
            {
                a.Delete();
            }

            Library library = new Library(TestFolder);
            Log("Adding new book...");
            LibraryHelper.AddEpubFromResource("Test/epub/aliceDynamic.epub", library);

            // Navigate
            navigation.Navigate(new Uri(
                "/Source/Pages/PaginationTestPage.xaml?" +
                "chapterNumber=" + chapterNumber + "&" +
                "pageCount=" + expectedPageCount, UriKind.Relative));
        }
    }
}
