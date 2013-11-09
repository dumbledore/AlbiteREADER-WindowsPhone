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

        private SpineElement[] spine;
        // TODO: ToC
        // TODO: Lists

        public BookPresenter(Book book)
        {
            this.Book = book;

            using (AlbiteIsolatedContainer iso = new AlbiteIsolatedContainer(ContentPath))
            {
                // All installed books are in ePub
                using (BookContainer container
                    = BookContainer.GetContainer(iso, BookContainerType.Epub))
                {
                    spine = prepareSpine(container);
                }

                bookLocation = prepareLocation();
            }

            // TODO: Get the current location from the database
        }

        private SpineElement[] prepareSpine(BookContainer container)
        {
            List<SpineElement> spine = new List<SpineElement>();
            SpineElement previous = null;
            SpineElement current = null;
            int number = 0;

            foreach (string url in container.Spine)
            {
                // Add the chapter to the spine
                current = new SpineElement(
                        Book,
                        number++,
                        url,
                        previous
                );
                spine.Add(current);
                previous = current;
            }

            return spine.ToArray();
        }

        public IList<SpineElement> Spine
        {
            get
            {
                return Array.AsReadOnly(spine);
            }
        }

        private BookLocation prepareLocation()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext())
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
            if (spineIndex < 0 || spineIndex >= spine.Length)
            {
                throw new EntityInvalidException("Spine value is out of range");
            }

            DomLocation domLocation =
                DomLocation.FromString(domLocationString);

            SpineElement element = spine[spineIndex];
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
                if (value.SpineElement.Book != Book)
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

                bookEntity.SpineIndex = bookLocation.SpineElement.Number;
                bookEntity.DomLocation = domLocation;

                dc.SubmitChanges();
            }
        }

        // Helper methods
        public string Path
        {
            get { return Book.Library.Books.GetPath(Book); }
        }

        public string RelativeContentPath
        {
            get { return BookManager.RelativeContentPath; }
        }

        public string ContentPath
        {
            get { return Book.Library.Books.GetContentPath(Book); }
        }

        public string RelativeEnginePath
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