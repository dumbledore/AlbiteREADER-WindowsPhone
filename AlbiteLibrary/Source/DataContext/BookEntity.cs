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
        public string Title { get; internal set; }

        // Author (reference)
        // Genre (reference)
        // Subjects (reference)

        // Reading Persistance
        [Column]
        public int SpineIndex { get; set; }

        [Column]
        public string DomLocation { get; set; }

        [Column]
        public int TextLocation { get; set; }

        // Notes
        private EntitySet<NoteEntity> notesSet = new EntitySet<NoteEntity>();

        [Association(Storage = "notesSet", OtherKey = "bookId")]
        public EntitySet<NoteEntity> Notes
        {
            get { return notesSet; }
            set { notesSet.Assign(value); }
        }
    }
}
