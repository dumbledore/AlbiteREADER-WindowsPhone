using SvetlinAnkov.Albite.BookLibrary.Location;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SvetlinAnkov.Albite.BookLibrary.DataContext
{
    [Table(Name = "Bookmarks")]
    internal class BookmarkEntity : Entity
    {
        // For DC
        public BookmarkEntity() { }

        // When creating a new one through BookmarkManager
        public BookmarkEntity(BookEntity bookEntity,
            BookLocation bookLocation, string text)
        {
            bookId = bookEntity.Id;
            Location = bookLocation.ToString();
            Text = text;
        }

        [Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        internal int MappedId { get; private set; }

        public override int Id
        {
            get { return MappedId; }
            protected set { MappedId = value; }
        }

        [Column(IsPrimaryKey = true)]
        internal int bookId;

        [Column]
        public string Location { get; set; }

        [Column]
        public string Text { get; set; }
    }
}
