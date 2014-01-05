using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SvetlinAnkov.Albite.BookLibrary.DataContext
{
    [Table(Name = "Books")]
    internal class BookEntity : Entity
    {
        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        internal int MappedId { get; private set; }

        public override int Id
        {
            get { return MappedId; }
            protected set { MappedId = value; }
        }

        [Column]
        public byte[] Hash { get; internal set; }

        [Column]
        public string Title { get; internal set; }

        [Column]
        public string Author { get; internal set; }

        // Reading Persistance
        [Column]
        public string HistoryStack;
    }
}
