using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using SvetlinAnkov.Albite.READER.Model.Container;
using System.Diagnostics.CodeAnalysis;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;
using SvetlinAnkov.Albite.READER.Model.Container.Epub;

namespace SvetlinAnkov.Albite.READER.Model
{
    public class Library : IDisposable
    {
        // Public API
        public BookManager Books { get; private set; }

        public delegate void PersistDelegate();

        // Private Implementation

        // Data Base
        private LibraryDataContext db;

        private PersistDelegate persistDelegate;

        public string LibraryPath { get; private set; }
        private string dbPath;

        public Library(string libraryPath)
        {
            LibraryPath = libraryPath;
            dbPath = Path.Combine(LibraryPath, "Database.sdf");

            persistDelegate = new PersistDelegate(persist);

            using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(dbPath))
            {
                s.CreatePathForFile();
            }

            db = new LibraryDataContext(dbPath);
            if (!db.DatabaseExists())
            {
                db.CreateDatabase();
            }

            Books = new BookManager(this);
        }

        private void persist()
        {
            lock (db)
            {
                db.SubmitChanges();
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public class BookManager
        {
            private static readonly string tag = "BookManager";

            private Library library;
            private string booksPath;
            private string booksTempPath;

            public BookManager(Library library)
            {
                this.library = library;
                booksPath = Path.Combine(library.LibraryPath, "Books");
                booksTempPath = Path.Combine(booksPath, "Temp");
            }

            public Book Add(Book.Descriptor descriptor)
            {
                BookContainer bookContainer = descriptor.GetContainer();

                Book book = new Book();

                // Fill in the defaults so that if there's
                // a problem with the metadata it would
                // fail gracefully.
                book.Title = bookContainer.Title;

                //TODO: Fill in the metadata,
                //incl author and info from Freebase.

                // TODO: Check whether a book with the same metadata and
                // SHA already exists in the database.

                try
                {
                    // Remove the temp folder
                    using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(booksTempPath))
                    {
                        s.Delete();
                    }

                    // Unpack
                    bookContainer.Install(booksTempPath);

                    lock (library.db)
                    {

                        // Add to the database
                        library.db.Books.InsertOnSubmit(book);

                        // If there's an error, with the database
                        // it will roll back the changes
                        // and thrown an Exception
                        library.db.SubmitChanges();

                        // Create the chapters
                        List<string> urls = new List<string>();

                        foreach (string url in bookContainer.Spine)
                        {
                            if (urls.Contains(url))
                            {
                                Log.W(tag, "Chapter " + url + " already there");
                                continue;
                            }

                            urls.Add(url);

                            // Add it to the book at last
                            book.Chapters.Add(new Chapter(book, url));
                        }

                        // Submit again, this time the chapters
                        library.db.SubmitChanges();
                    }
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
                    using (AlbiteIsolatedStorage s = new AlbiteIsolatedStorage(booksTempPath))
                    {
                        s.Move(GetContentPath(book));
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
                get
                {
                    lock (library.db)
                    {
                        return library.db.Books.Single(b => b.Id == id);
                    }
                }
            }

            public Book.Presenter GetPresenter(Book book)
            {
                return new Book.Presenter(
                    book,
                    new EpubContainer(
                        new AlbiteIsolatedContainer(GetContentPath(book))),
                    this,
                    new PersistDelegate(library.persist));
            }

            public static string RelativeContentPath
            {
                get { return "content"; }
            }

            public static string RelativeEnginePath
            {
                get { return "albite"; }
            }

            public string GetPath(Book book)
            {
                return Path.Combine(booksPath, book.Id.ToString());
            }

            public string GetContentPath(Book book)
            {
                return Path.Combine(GetPath(book), RelativeContentPath);
            }

            public string GetEnginePath(Book book)
            {
                return Path.Combine(GetPath(book), RelativeEnginePath);
            }

            // TODO: Simplified API for querying the database
        }

        private class LibraryDataContext : DataContext
        {
            public Table<Book> Books;
            public Table<Chapter> Chapters;

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
