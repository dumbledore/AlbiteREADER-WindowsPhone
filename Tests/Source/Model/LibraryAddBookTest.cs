using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER.Model;
using System.IO;
using SvetlinAnkov.Albite.BookLibrary;

namespace SvetlinAnkov.Albite.Tests.Model
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
