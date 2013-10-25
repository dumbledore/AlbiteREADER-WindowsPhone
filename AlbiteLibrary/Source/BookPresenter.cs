using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.Library.DataContext;

namespace SvetlinAnkov.Albite.Library
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

        private Location prepareLocation()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext())
            {
                BookEntity bookEntity = getEntity(dc);
                return CreateLocation(
                    bookEntity.SpineIndex,
                    bookEntity.DomLocation,
                    bookEntity.TextLocation);
            }
        }

        internal Location CreateLocation(
            int spineIndex, string domLocation, int textLocation)
        {
            if (spineIndex < 0 || spineIndex >= spine.Length)
            {
                throw new EntityInvalidException("Spine value is out of range");
            }

            SpineElement element = spine[spineIndex];
            return element.CreateLocation(
                domLocation, textLocation);
        }

        private Location bookLocation;
        public Location BookLocation
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

        public void Persist()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext())
            {
                BookEntity bookEntity = getEntity(dc);

                bookEntity.SpineIndex = bookLocation.SpineElement.Number;
                bookEntity.TextLocation = bookLocation.TextLocation;
                bookEntity.DomLocation = bookLocation.DomLocation;

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

        public class SpineElement
        {
            public Book Book { get; private set; }
            public int Number { get; private set; }
            public string Url { get; private set; }
            public SpineElement Previous { get; private set; }
            public SpineElement Next { get; private set; }

            internal SpineElement(
                Book book,
                int number,
                string url,
                SpineElement previous,
                SpineElement next = null)
            {
                Book = book;
                Number = number;
                Url = url;
                Previous = previous;
                Next = next;

                if (previous != null)
                {
                    previous.Next = this;
                }

                if (next != null)
                {
                    next.Previous = this;
                }
            }

            public Location CreateLocation(string domLocation, int textLocation)
            {
                Location location = new Location(this, domLocation, textLocation);
                return location;
            }
        }

        public class Location
        {
            public SpineElement SpineElement { get; private set; }
            public string DomLocation { get; private set; }
            public int TextLocation { get; private set; }

            internal Location(
                SpineElement spineElement,
                string domLocation,
                int textLocation)
            {
                SpineElement = spineElement;
                DomLocation = domLocation;
                TextLocation = textLocation;
            }
        }

        private BookEntity getEntity(LibraryDataContext dc)
        {
            return BookManager.GetEntity(dc, Book.Id);
        }
    }
}