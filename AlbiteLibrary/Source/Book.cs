using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.Container;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Book : LibraryEntity
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
    }
}
