using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.BookLibrary.Location;
using System;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Bookmark : LibraryEntity, IComparable<Bookmark>
    {
        public BookmarkManager Manager { get; private set; }

        public BookLocation BookLocation { get; private set; }

        public string Text { get; private set; }

        internal Bookmark(BookmarkManager manager, BookmarkEntity entity)
            : base(manager.Library, entity)
        {
            Manager = manager;

            Text = entity.Text;

            // Set up the location
            BookLocation = BookLocation.FromString(entity.Location);
            BookLocation.Attach(manager.BookPresenter);
        }

        public int CompareTo(Bookmark other)
        {
            return BookLocation.CompareTo(other.BookLocation);
        }
    }
}
