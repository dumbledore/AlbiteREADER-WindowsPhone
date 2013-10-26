using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.Test;

namespace SvetlinAnkov.Albite.BookLibrary.Test
{
    public class LibraryAddBookTest : TestCase
    {
        private string location;
        private Book.Descriptor[] books;

        public LibraryAddBookTest(string location, Book.Descriptor[] books)
        {
            this.location = location;
            this.books = books;
        }

        protected override void TestImplementation()
        {
            // Start using the library
            BookLibrary.Library library = new BookLibrary.Library(location);
            foreach (Book.Descriptor book in books)
            {
                // Add a book
                library.Books.Add(book);
            }
        }
    }
}
