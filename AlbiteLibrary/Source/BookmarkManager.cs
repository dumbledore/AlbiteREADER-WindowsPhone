using SvetlinAnkov.Albite.BookLibrary.DataContext;
using SvetlinAnkov.Albite.BookLibrary.Location;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class BookmarkManager : EntityManager<Bookmark>
    {
        public BookPresenter BookPresenter { get; private set; }

        internal BookmarkManager(BookPresenter bookPresenter)
            : base(bookPresenter.Book.Library)
        {
            BookPresenter = bookPresenter;
        }

        public Bookmark Add(BookLocation location, string text)
        {
            using (LibraryDataContext dc = Library.GetDataContext())
            {
                BookEntity bookEntity = getBookEntity(dc);

                BookmarkEntity bookmarkEntity;

                // Check if a bookmark for this location already exists,
                // i.e. a bookmark for the same book id
                // having the same location json string
                IQueryable<BookmarkEntity> entities =
                dc.Bookmarks.Where(
                    n => n.bookId == BookPresenter.Book.Id
                        && n.Location == location.ToString());

                if (entities.Count() > 0)
                {
                    // Will return the already existing one,
                    // no need to create a new bookmark
                    // for the *very* same location
                    bookmarkEntity = entities.First();
                }
                else
                {
                    // Create the entity
                    bookmarkEntity = new BookmarkEntity(bookEntity, location, text);

                    // Insert to the entity table
                    dc.Bookmarks.InsertOnSubmit(bookmarkEntity);

                    // Commit changes to DB
                    dc.SubmitChanges();
                }

                // Ready to return the bookmark
                return new Bookmark(this, bookmarkEntity);
            }
        }

        public override void Remove(Bookmark entity)
        {
            throw new NotImplementedException();
        }

        public override Bookmark this[int id]
        {
            get
            {
                using (LibraryDataContext dc = Library.GetDataContext(true))
                {
                    BookmarkEntity bookmarkEntity = dc.Bookmarks.Single(
                        n => n.MappedId == id && n.bookId == BookPresenter.Book.Id);
                    return new Bookmark(this, bookmarkEntity);
                }
            }
        }

        public override Bookmark[] GetAll()
        {
            using (LibraryDataContext dc = Library.GetDataContext(true))
            {
                BookmarkEntity[] bookmarkEntities =
                    dc.Bookmarks.Where(
                        n => n.bookId == BookPresenter.Book.Id).ToArray();

                Bookmark[] bookmarks = new Bookmark[bookmarkEntities.Length];

                for (int i = 0; i < bookmarks.Length; i++)
                {
                    bookmarks[i] = new Bookmark(this, bookmarkEntities[i]);
                }

                return bookmarks;
            }
        }

        private BookEntity getBookEntity(LibraryDataContext dc)
        {
            return BookManager.GetEntity(dc, BookPresenter.Book.Id);
        }
    }
}
