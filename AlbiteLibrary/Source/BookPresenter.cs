using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class BookPresenter
    {
        public Book Book { get; private set; }

        public Spine Spine { get; private set; }
        // TODO: ToC
        // TODO: Lists

        public BookPresenter(Book book)
        {
            this.Book = book;
            Spine = prepareSpine();
            bookLocation = prepareLocation();
        }

        private Spine prepareSpine()
        {
            using (AlbiteIsolatedContainer iso = new AlbiteIsolatedContainer(ContentPath))
            {
                // All installed books are in ePub
                using (BookContainer container
                    = BookContainer.GetContainer(iso, BookContainerType.Epub))
                {
                    return Spine.Create(this, container);
                }
            }
        }

        private BookLocation prepareLocation()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext(true))
            {
                BookEntity bookEntity = getEntity(dc);

                BookLocation bookLocation;

                // Deserialize
                if (bookEntity.Location != null
                    && bookEntity.Location != string.Empty)
                {
                    bookLocation = BookLocation.FromString(bookEntity.Location);

                    // Attach to the context
                    bookLocation.Attach(this);
                }
                else
                {
                    // Not a valid string. Perhaps, it's the first time the book has
                    // been opened. Default to the first page of the first chapter.
                    bookLocation = Spine[0].CreateLocation(DomLocation.Default);

                    // No need to attach as the created location is already attached
                }


                return bookLocation;
            }
        }

        private BookLocation bookLocation;

        /// <summary>
        /// Get/set the persisted location.
        /// Note: Call Persist to actually
        /// write the data to the library
        /// </summary>
        public BookLocation BookLocation
        {
            get
            {
                return bookLocation;
            }
            set
            {
                if (value.Chapter.BookPresenter.Book.Id != Book.Id)
                {
                    throw new EntityInvalidException(
                        "Bad location: Spine is not from this book");
                }

                bookLocation = value;
            }
        }

        /// <summary>
        /// Persist the location to the library
        /// </summary>
        public void Persist()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext())
            {
                BookEntity bookEntity = getEntity(dc);
                bookEntity.Location = bookLocation.ToString();

                dc.SubmitChanges();
            }
        }

        // Helper methods
        public string Path
        {
            get { return Book.Library.Books.GetPath(Book); }
        }

        public static string RelativeContentPath
        {
            get { return BookManager.RelativeContentPath; }
        }

        public string ContentPath
        {
            get { return Book.Library.Books.GetContentPath(Book); }
        }

        public static string RelativeEnginePath
        {
            get { return BookManager.RelativeEnginePath; }
        }

        public string EnginePath
        {
            get { return Book.Library.Books.GetEnginePath(Book); }
        }

        private BookEntity getEntity(LibraryDataContext dc)
        {
            return BookManager.GetEntity(dc, Book.Id);
        }
    }
}