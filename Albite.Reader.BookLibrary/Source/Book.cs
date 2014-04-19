using Albite.Reader.BookLibrary.DataContext;
using Albite.Reader.Container;
using System;

namespace Albite.Reader.BookLibrary
{
    public class Book : LibraryEntity, IComparable<Book>
    {
        public BookManager Manager { get; private set; }

        public string Title { get; private set; }

        public string Author { get; private set; }

        internal Book(BookManager manager, BookEntity entity)
            : base(manager.Library, entity)
        {
            Manager = manager;

            // Other entity fields
            Title = entity.Title;
            Author = entity.Author;
        }

        public int CompareTo(Book other)
        {
            return Title.CompareTo(other.Title);
        }
    }
}
