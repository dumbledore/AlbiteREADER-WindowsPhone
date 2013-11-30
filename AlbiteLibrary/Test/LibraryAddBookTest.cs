﻿using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.Test;

namespace SvetlinAnkov.Albite.BookLibrary.Test
{
    public class LibraryAddBookTest : TestCase
    {
        private string location;
        private string[] books;

        public LibraryAddBookTest(string location, string[] books)
        {
            this.location = location;
            this.books = books;
        }

        protected override void TestImplementation()
        {
            // Start using the library
            BookLibrary.Library library = new BookLibrary.Library(location);
            foreach (string book in books)
            {
                // Add a book
                LibraryHelper.AddEpubFromResource(book, library);
            }
        }
    }
}
