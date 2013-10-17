using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Collections.Generic;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.Container;

namespace SvetlinAnkov.Albite.Library
{
    [Table(Name = "Books")]
    public class Book : Entity
    {
        // ID
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        public override int Id { get; protected set; }

        [Column]
        public string Title { get; internal set; }

        // Author (reference)

        // Genre (reference)

        // Subjects (reference)

        // SHA-512 so that one wouldn't accidentally
        // add the same book twice.

        // Download URL so that one would be able to
        // easily download the contents again.
        //
        // This might include some special protocol name
        // for non http links, e.g.:
        //
        //   myuser@dropbox://mybooks/book.epub
        //   myuser@skydrive://mybooks/book.epub

        // Archived (boolean)

        // TODO: Add times
        // 1. Date added
        // 2. Date last accessed

        // Reading Persistance
        [Column]
        internal int SpineIndex { get; set; }

        [Column]
        internal string DomLocation { get; set; }

        [Column]
        internal int TextLocation { get; set; }

        // Notes
        private EntitySet<Note> notesSet = new EntitySet<Note>();

        [Association(Storage="notesSet", OtherKey="bookId")]
        internal EntitySet<Note> Notes
        {
            get { return notesSet; }
            //set { notesSet.Assign(value); }
        }

        internal Book() { }

        public override void Remove()
        {
            Library.Books.Remove(this);
        }

        public override void Persist()
        {
            // TODO
        }

        public BookPresenter GetPresenter()
        {
            return new BookPresenter(this);
        }

        public sealed class Descriptor
        {
            public string Path { get; private set; }
            public BookContainerType Type { get; private set; }

            public Descriptor(string path, BookContainerType type)
            {
                Path = path;
                Type = type;
            }
        }
    }
}
