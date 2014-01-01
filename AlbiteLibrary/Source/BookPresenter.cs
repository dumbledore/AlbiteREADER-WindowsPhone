using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Container.Epub;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class BookPresenter
    {
        public Book Book { get; private set; }

        public BookmarkManager BookmarkManager { get; private set; }

        public Spine Spine { get; private set; }
        // TODO: ToC
        // TODO: Lists

        private string cover;

        public BookPresenter(Book book)
        {
            Book = book;
            Spine = prepareSpine();
            bookLocation = prepareLocation();
            BookmarkManager = new BookmarkManager(this);
        }

        private Spine prepareSpine()
        {
            using (AlbiteIsolatedContainer iso = new AlbiteIsolatedContainer(ContentPath))
            {
                // All installed books are in ePub
                using (BookContainer container = new EpubContainer(iso))
                {
                    // cache the cover path
                    cover = container.Cover;

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
                    bookLocation = Spine[0].CreateLocation(ChapterLocation.Default);

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

        public Stream GetCoverStream()
        {
            Stream stream = null;

            if (cover != null)
            {
                try
                {
                    using (AlbiteIsolatedContainer iso = new AlbiteIsolatedContainer(ContentPath))
                    {
                        stream = iso.Stream(cover);
                    }
                }
                catch { }
            }

            return stream;
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