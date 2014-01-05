using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Container.Epub;
using SvetlinAnkov.Albite.Core.Collections;
using SvetlinAnkov.Albite.Core.IO;
using System.IO;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class BookPresenter
    {
        public Book Book { get; private set; }

        public BookmarkManager BookmarkManager { get; private set; }

        public Spine Spine { get; private set; }

        public ITree<IContentItem> Contents { get; private set; }

        private string cover;

        public BookPresenter(Book book)
        {
            // Set book reference
            Book = book;

            // Create the book manager
            BookmarkManager = new BookmarkManager(this);

            // Process data from book container
            processContainer();

            // Retrieve the current history
            history = prepareHistoryStack();
        }

        private void processContainer()
        {
            // Get data from the book container
            using (AlbiteIsolatedContainer iso = new AlbiteIsolatedContainer(ContentPath))
            {
                // All installed books are in ePub
                using (BookContainer container = new EpubContainer(iso))
                {
                    // cache the cover path
                    cover = container.Cover;

                    // cache the contents
                    Contents = container.Contents;

                    // create the spine from the container
                    Spine = Spine.Create(this, container);
                }
            }
        }

        private HistoryStack prepareHistoryStack()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext(true))
            {
                BookEntity bookEntity = getEntity(dc);

                HistoryStack history;

                try
                {
                    // Deserialize
                    history = HistoryStack.FromString(bookEntity.HistoryStack);

                    // Attach
                    history.Attach(this);
                }
                catch
                {
                    history = new HistoryStack(this);
                }

                return history;
            }
        }

        private HistoryStack history;

        public HistoryStack HistoryStack
        {
            get { return history; }
        }

        /// <summary>
        /// Persist the history stack to the library
        /// </summary>
        public void Persist()
        {
            using (LibraryDataContext dc = Book.Library.GetDataContext())
            {
                BookEntity bookEntity = getEntity(dc);
                bookEntity.HistoryStack = history.ToString();

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