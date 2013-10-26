using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.Container;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Book : LibraryEntity
    {
        public BookManager Manager { get; private set; }

        public string Title { get; private set; }

        internal Book(BookManager manager, BookEntity entity)
            : base(manager.Library, entity)
        {
            Manager = manager;

            // Other entity fields
            Title = entity.Title;
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
