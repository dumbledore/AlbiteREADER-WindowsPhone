﻿using SvetlinAnkov.Albite.BookLibrary.Location;
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