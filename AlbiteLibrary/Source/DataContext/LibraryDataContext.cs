using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library.DataContext
{
    internal class LibraryDataContext : System.Data.Linq.DataContext
    {
        public Table<Book> Books;
        public Table<Note> Notes;

        public LibraryDataContext(string fileOrConnection)
            : base(fileOrConnection) { }
    }
}
