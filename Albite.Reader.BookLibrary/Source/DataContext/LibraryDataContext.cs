using System.Data.Linq;

namespace Albite.Reader.BookLibrary.DataContext
{
    internal class LibraryDataContext : System.Data.Linq.DataContext
    {
#pragma warning disable 0649
        public Table<BookEntity> Books;
        public Table<BookmarkEntity> Bookmarks;
#pragma warning restore 0649

        public LibraryDataContext(string fileOrConnection, bool readOnly)
            : base(fileOrConnection)
        {
            ObjectTrackingEnabled = !readOnly;
        }
    }
}
