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
using SvetlinAnkov.Albite.READER.Model.Container.Epub;

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
        private string locationString { get; set; }



        public class Presenter
        {
            private readonly Library.BookManager manager;
            private readonly Library.PersistDelegate persist;

            public Book Book { get; private set; }

            private SpineElement[] spine;
            // TODO: ToC
            // TODO: Lists

            internal Presenter(
                Book book,
                BookContainer container,
                Library.BookManager manager,
                Library.PersistDelegate persist)
            {
                this.Book = book;
                this.manager = manager;
                this.persist = persist;

                prepare(container);
            }

            private void prepare(BookContainer container)
            {
                // Load all the spine elements
                spine = prepareSpine(container);

                // Set bookLocation
                bookLocation = new BookLocation(
                    spine[Book.currentChapterIndex],
                    Book.locationString);
            }

            private SpineElement[] prepareSpine(BookContainer container)
            {
                List<SpineElement> spine = new List<SpineElement>();
                SpineElement previous = null;
                SpineElement current = null;
                int number = 0;

                foreach (string url in container.Spine)
                {
                    // Add the chapter to the spine
                    current = new SpineElement(number++,
                            url,
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
                    Book.locationString = bookLocation.DomLocation;
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
        }

        public class SpineElement
        {
            public int Number { get; private set; }
            public string Url { get; private set; }
            public SpineElement Previous { get; private set; }
            public SpineElement Next { get; private set; }

            internal SpineElement(
                int number, string url,
                SpineElement previous, SpineElement next)
            {
                Number = number;
                Url = url;
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
            public string DomLocation { get; private set; }

            public BookLocation(SpineElement spineElement, string domLocation)
            {
                SpineElement = spineElement;
                DomLocation = domLocation;
            }
        }

        public enum ContainerType
        {
            Epub,
        }

        public sealed class ContainerDescriptor : IDisposable
        {
            public IAlbiteContainer Container { get; private set; }
            public ContainerType Type { get; private set; }

            public ContainerDescriptor(
                IAlbiteContainer container, ContainerType type)
            {
                Container = container;
                Type = type;
            }

            internal BookContainer GetContainer()
            {
                switch (Type)
                {
                    case ContainerType.Epub:
                        return new EpubContainer(Container);
                }

                throw new InvalidOperationException("Unknown container type");
            }

            public void Dispose()
            {
                Container.Dispose();
            }
        }

        public sealed class PathDescriptor
        {
            public string Path { get; private set; }
            public ContainerType Type { get; private set; }

            public PathDescriptor(string path, ContainerType type)
            {
                Path = path;
                Type = type;
            }
        }
    }
}
