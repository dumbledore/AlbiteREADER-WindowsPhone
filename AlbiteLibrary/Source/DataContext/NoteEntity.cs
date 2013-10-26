using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SvetlinAnkov.Albite.BookLibrary.DataContext
{
    [Table(Name = "Notes")]
    internal class NoteEntity : Entity
    {
        // For DC
        public NoteEntity() { }

        // When creating a new one through NoteManager
        public NoteEntity(BookEntity bookEntity,
            BookPresenter.Location location, string text)
        {
            Book = bookEntity;
            SpineIndex = location.SpineElement.Number;
            DomLocation = location.DomLocation;
            TextLocation = location.TextLocation;
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
        private int bookId;
        private EntityRef<BookEntity> bookRef;

        [Association(Storage = "bookRef", ThisKey = "bookId")]
        public BookEntity Book
        {
            get { return bookRef.Entity; }
            private set
            {
                bookRef.Entity = value;
                // There seems to be a bug on WP7 and
                // one needs to set the ID explicitly.
                bookId = value.Id;
            }
        }

        [Column]
        public int SpineIndex { get; set; }

        [Column]
        public string DomLocation { get; set; }

        [Column]
        public int TextLocation { get; set; }

        [Column]
        public string Text { get; set; }
    }
}
