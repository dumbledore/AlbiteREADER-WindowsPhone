using System.Data.Linq;

namespace SvetlinAnkov.Albite.BookLibrary.DataContext
{
    internal class LibraryDataContext : System.Data.Linq.DataContext
    {
        public Table<BookEntity> Books;
        public Table<NoteEntity> Notes;

        public LibraryDataContext(string fileOrConnection)
            : base(fileOrConnection) { }
    }
}
