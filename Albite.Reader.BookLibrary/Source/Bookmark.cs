﻿using Albite.Reader.BookLibrary.DataContext;
using Albite.Reader.BookLibrary.Location;

namespace Albite.Reader.BookLibrary
{
    public class Bookmark : LibraryEntity, IBookmark
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

        public int CompareTo(IBookmark other)
        {
            return BookLocation.CompareTo(other.BookLocation);
        }
    }
}
