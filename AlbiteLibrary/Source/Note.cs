using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace SvetlinAnkov.Albite.Library
{
    /// <summary>
    /// An XHTML resource from a book.
    /// </summary>
    [Table(Name = "Notes")]
    public class Note
    {
        // ID
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        public int Id { get; private set; }

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
        public string Text { get; set; }

        [Column(Name = "SpineIndex")]
        private int spineIndex { get; set; }

        [Column(Name = "DomLocation")]
        private string domLocation { get; set; }

        // Used by LinqToSql for deserialization
        internal Note() { }

        // Used when creating a new entity
        public Note(Book book, BookPresenter.Location location, string text)
        {
            Book = book;
            spineIndex = location.SpineElement.Number;
            domLocation = location.DomLocation;
            Text = text;
        }

        public BookPresenter.Location GetBookLocation(BookPresenter presenter)
        {
            if (presenter.Book.Id != Book.Id)
            {
                throw new InvalidOperationException();
            }

            return null;
        }
        // TODO: Chapter Bookmarks, Highlights and Notes
    }
}
