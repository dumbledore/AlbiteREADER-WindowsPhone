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

                return CreateLocation(
                    bookEntity.SpineIndex,
                    bookEntity.DomLocation);
            }
        }

        internal BookLocation CreateLocation(
            int spineIndex, string domLocationString)
        {
            if (spineIndex < 0 || spineIndex >= Spine.Length)
            {
                throw new EntityInvalidException("Spine value is out of range");
            }

            DomLocation domLocation =
                DomLocation.FromString(domLocationString);

            Chapter element = Spine[spineIndex];
            return element.CreateLocation(domLocation);
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
            string domLocation = bookLocation.DomLocation.ToString();

            using (LibraryDataContext dc = Book.Library.GetDataContext())
            {
                BookEntity bookEntity = getEntity(dc);

                bookEntity.SpineIndex = bookLocation.Chapter.Number;
                bookEntity.DomLocation = domLocation;

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