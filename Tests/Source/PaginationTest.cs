using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Test;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Core.Test;
using System;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.Tests
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
            using (AlbiteIsolatedStorage a = new AlbiteIsolatedStorage(TestFolder))
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
