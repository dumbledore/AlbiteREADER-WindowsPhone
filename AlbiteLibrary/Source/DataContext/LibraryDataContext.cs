using System.Data.Linq;

namespace SvetlinAnkov.Albite.BookLibrary.DataContext
{
    internal class LibraryDataContext : System.Data.Linq.DataContext
    {
        public Table<BookEntity> Books;
        public Table<BookmarkEntity> Bookmarks;

        public LibraryDataContext(string fileOrConnection)
            : base(fileOrConnection) { }
    }
}
