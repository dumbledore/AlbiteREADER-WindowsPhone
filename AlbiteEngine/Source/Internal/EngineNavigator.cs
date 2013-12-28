using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using System;

namespace SvetlinAnkov.Albite.Engine.Internal
{
    internal class EngineNavigator : IEngineNavigator
    {
        private static readonly string tag = "BookEngine";

        protected readonly AlbiteEngine engine;

        private Chapter current;

        // TODO: Add history stack

        public EngineNavigator(AlbiteEngine engine)
        {
            this.engine = engine;
        }

        internal ChapterLocation ChapterLocation
        {
            get { return engine.Messenger.Location; }
            set { engine.Messenger.Location = value; }
        }

        public int Page
        {
            get { return engine.Messenger.Page; }
            set { engine.Messenger.Page = value; }
        }

        public int PageCount { get; internal set; }

        public void GoToFirstPage()
        {
            Page = 1;
        }

        public void GoToLastPage()
        {
            Page = PageCount;
        }

        public bool IsFirstChapter
        {
            get { return current.Previous == null; }
        }

        public bool IsLastChapter
        {
            get { return current.Next == null; }
        }

        public void GoToPreviousChapter()
        {
            if (IsFirstChapter)
            {
                Log.W(tag, "It's the first chapter already");
                return;
            }

            engine.TryPersist();

            current = current.Previous;
            engine.SetChapter(current.Url, new LastPageLocation());
        }

        public void GoToNextChapter()
        {
            if (IsLastChapter)
            {
                Log.W(tag, "It's the last chapter already");
                return;
            }

            engine.TryPersist();

            current = current.Next;
            engine.SetChapter(current.Url, new FirstPageLocation());
        }

        public BookLocation BookLocation
        {
            get
            {
                // TODO: What about when the current chapter
                // is not the current chapter, and thus is not
                return current.CreateLocation(ChapterLocation);
            }

            set
            {
                if (current == value.Chapter)
                {
                    // It's the same chapter, no need for reload
                    // Only updat the dom location
                    ChapterLocation = value.Location;
                }
                else
                {
                    engine.TryPersist();

                    current = value.Chapter;
                    engine.SetChapter(value.Chapter.Url, value.Location);
                }
            }
        }

        public Bookmark CreateBookmark()
        {
            // Get bookmark from client
            EngineMessenger.Bookmark clientBookmark
                = engine.Messenger.GetBookmark();

            // Create a BookLocation for the Bookmark
            BookLocation location = current.CreateLocation(clientBookmark.DomLocation);

            // Now create a new bookmark using BookmarkManager
            Bookmark bookmark = engine.BookPresenter.BookmarkManager.Add(location, clientBookmark.Text);

            // Done
            return bookmark;
        }
    }
}
