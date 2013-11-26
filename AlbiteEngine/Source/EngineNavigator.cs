using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using System;

namespace SvetlinAnkov.Albite.Engine
{
    public class EngineNavigator : IEngineNavigator
    {
        private static readonly string tag = "BookEngine";

        protected readonly AlbiteEngine engine;

        private Chapter current;

        // TODO: Add history stack

        public EngineNavigator(AlbiteEngine engine)
        {
            this.engine = engine;
        }

        internal DomLocation DomLocation
        {
            get { return engine.Messenger.DomLocation; }
            set { engine.Messenger.DomLocation = value; }
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
            engine.SetChapter(current.Url,
                InitialLocation.GetLastLocation());
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
            engine.SetChapter(current.Url,
                InitialLocation.GetFirstLocation());
        }

        public void GoToChapter(Chapter chapter, bool goToBeginning)
        {
            if (current == chapter)
            {
                // Same chapter, no need to reload.
                if (goToBeginning)
                {
                    GoToFirstPage();
                }
                else
                {
                    GoToLastPage();
                }
            }
            else
            {
                engine.TryPersist();

                current = chapter;
                engine.SetChapter(current.Url,
                    goToBeginning
                    ? InitialLocation.GetFirstLocation()
                    : InitialLocation.GetLastLocation()
                );
            }
        }

        public void GoToChapter(Chapter chapter, string fragment)
        {
            if (fragment.Length < 2 || !fragment.StartsWith("#"))
            {
                throw new ArgumentException("not a valid fragment");
            }

            if (current == chapter)
            {
                // Same chapter, no need to reload
                // Remove the Hash char first
                string elementId = fragment.Substring(1);
                engine.Messenger.GoToElementById(elementId);
            }
            else
            {
                engine.TryPersist();

                current = chapter;
                engine.SetChapter(current.Url,
                    InitialLocation.GetFragmentLocation(fragment));
            }
        }

        public BookLocation BookLocation
        {
            get
            {
                // TODO: What about when the current chapter
                // is not the current chapter, and thus is not
                return current.CreateLocation(DomLocation);
            }

            set
            {
                if (current == value.Chapter)
                {
                    // It's the same chapter, no need for reload
                    // Only updat the dom location
                    DomLocation = value.DomLocation;
                }
                else
                {
                    engine.TryPersist();

                    current = value.Chapter;
                    engine.SetChapter(
                        value.Chapter.Url,
                        InitialLocation.GetDomLocation(value.DomLocation));
                }
            }
        }
    }
}
