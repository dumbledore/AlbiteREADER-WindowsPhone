using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
