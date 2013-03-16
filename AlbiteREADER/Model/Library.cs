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
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using SvetlinAnkov.Albite.READER.Model.Container;
using System.Diagnostics.CodeAnalysis;

namespace SvetlinAnkov.Albite.READER.Model
{
    public class Library
    {
        // Public API
        public BookManager Books { get; private set; }

        // Private Implementation

        // Object lock
        private Object myLock = new Object();

        // Data Base
        private LibraryDataContext db;

        // Books Location
        private string booksLocation;

        public Library(string dbLocation, string booksLocation)
        {
            this.booksLocation = booksLocation;

            db = new LibraryDataContext(dbLocation);
            Books = new BookManager(this);
        }

        public class BookManager
        {
            private Library library;

            public BookManager(Library library)
            {
                this.library = library;
            }

            public Book Add(BookContainer container)
            {
                Book book = new Book();

                // Fill in the defaults so that if there's
                // a problem with the metadata it would
                // fail gracefully.
                book.Title = "New Book";

                //TODO: Fill in the metadata,
                //incl author and info from Freebase.

                try
                {
                    // Unpack
                    container.Install(null);

                    // Add to the database
                    library.db.Books.InsertOnSubmit(book);

                    // If there's an error, with the database
                    // it will roll back the changes
                    // and thrown an Exception
                    library.db.SubmitChanges();
                }
                catch (Exception e)
                {
                    // TODO: Try removing the storage

                    // Throw the error again so that it
                    // would be properly handled
                    throw e;
                }

                return book;
            }

            public void Delete(Book book)
            {
                // Delete the storage

                // Delete from the data base
            }

            public void Archive(Book book)
            {
                // TODO
                // This will delete the book
                // from the storage, but will retain
                // the book in the database, incl.
                // the necessary info to download it again.
                // The book will be available in the `archived`
                // section.
            }

            public Book this[int id]
            {
                get { return library.db.Books.Single(b => b.Id == id); }
            }

            // TODO: Simplified API for querying the database
        }

        private class LibraryDataContext : DataContext
        {
            public Table<Book> Books;

            public LibraryDataContext(string location) : base(location) { }
        }
    }
}
