using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace SvetlinAnkov.Albite.READER.Model
{

    /// <summary>
    /// An XHTML resource from a book.
    /// </summary>
    [Table(Name = "Chapters")]
    public class Chapter
    {
        // ID
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "bigint NOT NULL IDENTITY")]
        private Int64 id;

        [Column (IsPrimaryKey = true)]
        private int bookId;
        private EntityRef<Book> bookRef;

        [Association(Storage="bookRef", ThisKey = "bookId")]
        public Book Book
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
        public string Url { get; private set; }

        // Used by LinqToSql for deserialization
        public Chapter() { }

        // Used when creating a new entity
        public Chapter(Book book, string url)
        {
            Book = book;
            Url = url;
        }

        // TODO: Chapter Bookmarks, Highlights and Notes
    }
}
