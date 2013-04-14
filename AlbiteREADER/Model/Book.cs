using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Collections.Generic;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER.Model.Container;
using SvetlinAnkov.Albite.READER.BrowserEngine;

namespace SvetlinAnkov.Albite.READER.Model
{
    [Table(Name = "Books")]
    public class Book
    {
        // ID
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "int NOT NULL IDENTITY")]
        public int Id { get; private set; }

        [Column]
        public string Title { get; set; }

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

        // Reading Persistance

        [Column]
        private int currentChapterIndex { get; set; }

        [Column]
        private int locationIndex { get; set; }

        [Column]
        private int locationOffset { get; set; }

        // Chapters
        private EntitySet<Chapter> chaptersSet = new EntitySet<Chapter>();

        [Association(Storage="chaptersSet", OtherKey="bookId")]
        public EntitySet<Chapter> Chapters
        {
            get { return chaptersSet; }
            set { chaptersSet.Assign(value); }
        }

        public class Presenter : IDisposable
        {
            private readonly Book book;
            private readonly BookContainer container;
            private readonly Library.PersistDelegate persist;

            private readonly Object myLock = new Object();

            private SpineElement[] spine;
            // TODO: ToC
            // TODO: Lists
            // TODO: Export other metadata, perhaps?

            public Presenter(Book book, BookContainer container, Library.PersistDelegate persist)
            {
                this.book = book;
                this.container = container;
                this.persist = persist;

                prepare();
            }

            private void prepare()
            {
                // TODO
                // 1. Load all the spine elements
                // 2. Set bookLocation
            }

            private BookLocation bookLocation;
            public BookLocation BookLocation
            {
                get
                {
                    return bookLocation;
                }
                set
                {
                    bookLocation = value;
                    book.currentChapterIndex = bookLocation.SpineElement.Number;
                    book.locationIndex = bookLocation.DomLocation.ElementIndex;
                    book.locationOffset = bookLocation.DomLocation.TextOffset;
                }
            }

            public void Persist()
            {
                persist();
            }

            public void Dispose()
            {
                container.Dispose();
            }
        }

        public class SpineElement
        {
            public int Number { get; private set; }
            public Chapter Chapter { get; private set; }
            public SpineElement Previous { get; private set; }
            public SpineElement Next { get; private set; }

            internal SpineElement(
                int number, Chapter chapter,
                SpineElement previous, SpineElement next)
            {
                Number = number;
                Chapter = chapter;
                Previous = previous;
                Next = next;
            }
        }

        public class BookLocation
        {
            public SpineElement SpineElement { get; private set; }
            public DomLocation DomLocation { get; private set; }

            public BookLocation(SpineElement spineElement, DomLocation domLocation)
            {
                SpineElement = spineElement;
                DomLocation = domLocation;
            }
        }
    }
}
