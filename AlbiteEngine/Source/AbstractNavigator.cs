using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using System.IO;

namespace SvetlinAnkov.Albite.Engine
{
    public abstract class AbstractNavigator : IEngineNavigator
    {
        protected readonly AbstractEngine engine;

        public AbstractNavigator(AbstractEngine engine)
        {
            this.engine = engine;
        }

        public abstract BookLocation BookLocation { get; set; }

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

        public abstract bool IsFirstChapter { get; }
        public abstract bool IsLastChapter { get; }
        public abstract void GoToPreviousChapter();
        public abstract void GoToNextChapter();
    }
}
