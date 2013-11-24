using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;

namespace SvetlinAnkov.Albite.Engine
{
    public class EngineNavigator : IEngineNavigator
    {
        private static readonly string tag = "BookEngine";

        protected readonly AlbiteEngine engine;

        private SpineElement current;

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

        /// <summary>
        /// Go to a spine element
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="goToBeginning">If true, it goes to the first page of the chapter,
        /// otherwise it goes to the last</param>
        public void GoToChapter(SpineElement chapter, bool goToBeginning)
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

        /// <summary>
        /// Go to a SpineElement, specifying a hash string
        /// </summary>
        /// <param name="chapter">The chapter to go to</param>
        /// <param name="hash">The hash string, without the hash character</param>
        public void GoToChapter(SpineElement chapter, string hash)
        {
            if (current == chapter)
            {
                // Same chapter, no need to reload
                engine.Messenger.GoToElementById(hash);
            }
            else
            {
                engine.TryPersist();

                current = chapter;
                engine.SetChapter(current.Url,
                    InitialLocation.GetHashLocation(hash));
            }
        }

        public BookLocation BookLocation
        {
            get
            {
                // TODO: What about when the current spine element
                // is not the current chapter, and thus is not
                return current.CreateLocation(DomLocation);
            }

            set
            {
                if (current == value.SpineElement)
                {
                    // It's the same chapter, no need for reload
                    // Only updat the dom location
                    DomLocation = value.DomLocation;
                }
                else
                {
                    engine.TryPersist();

                    current = value.SpineElement;
                    engine.SetChapter(
                        value.SpineElement.Url,
                        InitialLocation.GetDomLocation(value.DomLocation));
                }
            }
        }
    }
}
