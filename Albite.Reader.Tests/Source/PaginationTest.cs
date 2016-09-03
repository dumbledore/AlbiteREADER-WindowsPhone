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

        private string _book;
        private int _chapterNumber;
        private int _expectedPageCount;
        private NavigationService _navigation;

        public PaginationTest(
            string book,
            int chapterNumber,
            int expectedPageCount,
            NavigationService navigation)
        {
            _book = book;
            _chapterNumber = chapterNumber;
            _expectedPageCount = expectedPageCount;
            _navigation = navigation;
        }

        protected override async void TestImplementation()
        {
            Log("Removing previous data");
            using (IsolatedStorage a = new IsolatedStorage(TestFolder))
            {
                a.Delete();
            }

            Library library = new Library(TestFolder);
            Log("Adding new book...");
            Book book = await LibraryHelper.AddEpubFromResource("Test/epub/aliceDynamic.epub", library);

            // Navigate
            _navigation.Navigate(new Uri(String.Format(
                "/Source/Pages/PaginationTestPage.xaml?chapterNumber={0}&pageCount={1}&id={2}",
                _chapterNumber, _expectedPageCount, book.Id), UriKind.Relative));
        }
    }
}
