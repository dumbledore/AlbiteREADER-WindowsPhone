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

            // Other fields
            BookLocation = manager.BookPresenter.CreateLocation(
                entity.SpineIndex,
                entity.DomLocation);

            Text = entity.Text;
        }

        public int CompareTo(Bookmark other)
        {
            return BookLocation.CompareTo(other.BookLocation);
        }
    }
}
