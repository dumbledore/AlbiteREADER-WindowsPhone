using Albite.Reader.BookLibrary.DataContext;
using Albite.Reader.Container;
using Albite.Reader.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.BookLibrary
{
    public class BookManager : EntityManager<Book>
    {
        private string booksPath;
        private string booksTempPath;

        internal BookManager(Library library)
            : base(library)
        {
            booksPath = Path.Combine(library.LibraryPath, "Books");
            booksTempPath = Path.Combine(booksPath, "Temp");
        }

        /// <summary>
        /// Adds a new book to the library.
        ///
        /// If there's a book with the same title & hash value,
        /// returns that book instead.
        /// </summary>
        /// <param name="bookContainer">BookContainer for the new book</param>
        /// <returns>Book from the library for this BookContainer</returns>
        public Book Add(BookContainer bookContainer)
        {
            return AddAsync(bookContainer, CancellationToken.None, null).Result;
        }

        public async Task<Book> AddAsync(BookContainer bookContainer,
            CancellationToken cancelToken, IProgress<double> progress)
        {
            // have we been cancelled?
            cancelToken.ThrowIfCancellationRequested();

            using (LibraryDataContext dc = Library.GetDataContext())
            {
                // Compute container's hash
                HashAlgorithm hashAlgorithm = new SHA1Managed();
                byte[] hash = bookContainer.ComputeHash(hashAlgorithm);

                // have we been cancelled?
                cancelToken.ThrowIfCancellationRequested();

                // Check if this book already exists
                IQueryable<BookEntity> bookEntities =
                    dc.Books.Where(b =>
                        b.Hash == hash &&
                        b.Title == bookContainer.Title);

                if (bookEntities.Count() > 0)
                {
                    // Book already available,
                    // no need to add it again
                    return new Book(this, bookEntities.First());
                }

                BookEntity bookEntity = new BookEntity();

                // Fill in the metadata
                bookEntity.Title = bookContainer.Title;
                bookEntity.Author = bookContainer.Author;

                // Fill in the hash
                bookEntity.Hash = hash;

                // have we been cancelled?
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    // Remove the temp folder
                    removeDirectory(booksTempPath);

                    // have we been cancelled?
                    cancelToken.ThrowIfCancellationRequested();

                    // Unpack
                    await bookContainer.InstallAsync(booksTempPath, cancelToken, progress);

                    // have we been cancelled?
                    cancelToken.ThrowIfCancellationRequested();

                    // Add to the database
                    dc.Books.InsertOnSubmit(bookEntity);

                    // If there's an error with the database,
                    // it will roll back the changes
                    // and thrown an Exception
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    // Remove from the storage
                    removeDirectory(booksTempPath);

                    // Throw the error again so that it
                    // would be properly handled
                    throw e;
                }

                // Working with the Book class now
                Book book = new Book(this, bookEntity);

                try
                {
                    // Move the book to the real folder
                    using (IsolatedStorage s = new IsolatedStorage(booksTempPath))
                    {
                        s.Move(GetContentPath(book));
                    }
                }
                catch (Exception e)
                {
                    // Move failed (very unlikely), still
                    // remove from the database as it was
                    // just added there.
                    Remove(book);

                    // Don't forget to throw the error
                    throw e;
                }

                return book;
            }
        }

        public override void Remove(Book book)
        {
            using (LibraryDataContext dc = Library.GetDataContext())
            {
                BookEntity bookEntity = GetEntity(dc, book.Id);

                // Remove from the storage
                // No need to catch exceptions,
                // they shall go up
                removeDirectory(GetPath(book));

                // Get all bookmarks for this book
                IQueryable<BookmarkEntity> bookmarkEntities =
                    dc.Bookmarks.Where(n => n.bookId == book.Id);

                // Queue those bookmarks for deletion
                dc.Bookmarks.DeleteAllOnSubmit(bookmarkEntities);

                // Remove from the data base
                // If there's an error with the database,
                // it will roll back the changes
                // and thrown an Exception
                dc.Books.DeleteOnSubmit(bookEntity);

                // Commit changes to the DB
                dc.SubmitChanges();
            }
        }

        public override Book this[int id]
        {
            get
            {
                using (LibraryDataContext dc = Library.GetDataContext(true))
                {
                    return new Book(this, GetEntity(dc, id));
                }
            }
        }

        public override Book[] GetAll()
        {
            using (LibraryDataContext dc = Library.GetDataContext(true))
            {
                int count = dc.Books.Count();
                List<Book> books = new List<Book>(count);

                foreach (BookEntity bookEntity in dc.Books)
                {
                    books.Add(new Book(this, bookEntity));
                }

                return books.ToArray();
            }
        }

        private void removeDirectory(string path)
        {
            using (IsolatedStorage s = new IsolatedStorage(path))
            {
                // Deleting a non-existent directory won't
                // throw an exception
                s.Delete();
            }
        }

        internal static BookEntity GetEntity(LibraryDataContext dc, int id)
        {
            return dc.Books.Single(b => b.MappedId == id);
        }

        public static string RelativeContentPath
        {
            get { return "content"; }
        }

        public static string RelativeEnginePath
        {
            get { return "engine"; }
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
    }
}
