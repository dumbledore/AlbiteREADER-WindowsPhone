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
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.Model
{
    public class Library : IDisposable
    {
        // Public API
        public BookManager Books { get; private set; }

        // Private Implementation

        // Object lock
        private Object myLock = new Object();

        // Data Base
        private LibraryDataContext db;

        public string LibraryPath { get; private set; }
        public string DbPath { get; private set; }
        public string BooksPath { get; private set; }
        private string booksTempPath;

        public Library(string libraryPath)
        {
            LibraryPath = libraryPath;
            DbPath = libraryPath + "/Database.sdf";
            BooksPath = libraryPath + "/Books";
            booksTempPath = BooksPath + "/Temp";

            using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(DbPath))
            {
                s.CreatePathForFile();
            }

            db = new LibraryDataContext(DbPath);
            if (!db.DatabaseExists())
            {
                db.CreateDatabase();
            }

            Books = new BookManager(this);
        }

        public void Dispose()
        {
            db.Dispose();
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
                book.Title = container.Title;

                //TODO: Fill in the metadata,
                //incl author and info from Freebase.

                // TODO: Check whether a book with the same metadata and
                // SHA already exists in the database.

                try
                {
                    // Remove the temp folder
                    using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(library.booksTempPath))
                    {
                        s.Delete();
                    }

                    // Unpack
                    container.Install(library.booksTempPath);

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

                try
                {
                    // Move the book to the real folder
                    using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(library.booksTempPath))
                    {
                        s.Move(library.BooksPath + "/" + book.Id);
                    }
                }
                catch (Exception e)
                {
                    // Move failed. TODO
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

            public LibraryDataContext(string location, int maxSize = 128) : base(getConnection(location, maxSize)) { }

            private static string getConnection(string location, int maxSize)
            {
                return string.Format(
                    "Data Source = 'isostore:/{0}'; Max Database Size = '{1}';",
                    location, maxSize);
            }
        }
    }
}
