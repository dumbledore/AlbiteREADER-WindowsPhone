using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Library.DataContext
{
    [Table(Name = "Books")]
    internal class BookEntity : Entity
    {
        public BookEntity() { }

        // ID
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        public override int Id { get; protected set; }

        [Column]
        public string Title { get; internal set; }

        // Author (reference)
        // Genre (reference)
        // Subjects (reference)

        // Reading Persistance
        [Column]
        internal int SpineIndex { get; set; }

        [Column]
        internal string DomLocation { get; set; }

        [Column]
        internal int TextLocation { get; set; }

        // Notes
        private EntitySet<NoteEntity> notesSet = new EntitySet<NoteEntity>();

        [Association(Storage = "notesSet", OtherKey = "bookId")]
        internal EntitySet<NoteEntity> Notes
        {
            get { return notesSet; }
            set { notesSet.Assign(value); }
        }
    }
}
