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
using SvetlinAnkov.Albite.READER.Model.Reader;

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
        private string locationPath { get; set; }

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

        public Chapter this[string url]
        {
            get
            {
                IEnumerable<Chapter> chapters = Chapters.Where(c => c.Url == url);

                if (chapters.Count() > 0)
                {
                    return chapters.First();
                }

                return null;
            }
        }

        public class Presenter : IDisposable
        {
            private readonly Library.BookManager manager;
            private readonly Library.PersistDelegate persist;

            public Book Book { get; private set; }
            public BookContainer Container { get; private set; }

            private readonly Object myLock = new Object();

            private SpineElement[] spine;
            // TODO: ToC
            // TODO: Lists

            public Presenter(
                Book book,
                BookContainer container,
                Library.BookManager manager,
                Library.PersistDelegate persist)
            {
                this.Book = book;
                this.Container = container;
                this.manager = manager;
                this.persist = persist;

                prepare();
            }

            private void prepare()
            {
                // Load all the spine elements
                spine = prepareSpine();

                // Set bookLocation
                bookLocation = new BookLocation(
                    spine[Book.currentChapterIndex],
                    new DomLocation(Book.locationPath, Book.locationOffset));
            }

            private SpineElement[] prepareSpine()
            {
                List<SpineElement> spine = new List<SpineElement>();
                SpineElement previous = null;
                SpineElement current = null;
                int number = 0;

                foreach (string url in Container.Spine)
                {
                    // Add the chapter to the spine
                    current = new SpineElement(number++,
                            Book[url],
                            previous,
                            null
                    );
                    spine.Add(current);
                    previous = current;
                }

                return spine.ToArray();
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
                    Book.currentChapterIndex = bookLocation.SpineElement.Number;
                    Book.locationPath = bookLocation.DomLocation.ElementPath;
                    Book.locationOffset = bookLocation.DomLocation.TextOffset;
                }
            }

            public string Path
            {
                get { return manager.GetPath(Book); }
            }

            public string RelativeContentPath
            {
                get { return Library.BookManager.RelativeContentPath; }
            }

            public string ContentPath
            {
                get { return manager.GetContentPath(Book); }
            }

            public string RelativeEnginePath
            {
                get { return Library.BookManager.RelativeEnginePath; }
            }

            public string EnginePath
            {
                get { return manager.GetEnginePath(Book); }
            }

            public void Persist()
            {
                persist();
            }

            public void Dispose()
            {
                Container.Dispose();
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

                if (previous != null)
                {
                    previous.Next = this;
                }

                if (next != null)
                {
                    next.Previous = this;
                }
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
