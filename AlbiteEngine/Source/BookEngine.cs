using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Engine.LayoutSettings;

namespace SvetlinAnkov.Albite.Engine
{
    public class BookEngine : AbstractEngine
    {
        private static readonly string tag = "BookEngine";

        public BookEngine(
            IEngineController controller, BookPresenter bookPresenter, Settings settings)
            : base(controller, bookPresenter, settings) { }

        protected override AbstractNavigator CreateNavigator()
        {
            return new BookNavigator(this);
        }

        private class BookNavigator : AbstractNavigator
        {
            public BookNavigator(BookEngine bookEngine)
                : base(bookEngine) { }

            // TODO: Add history stack

            SpineElement current;

            /// <summary>
            /// Setting this property would cause the engine
            /// to load into the specified book location
            /// </summary>
            public override BookLocation BookLocation
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
                        current = value.SpineElement;
                        engine.SetChapter(
                            value.SpineElement.Url,
                            InitialLocation.GetDomLocation(value.DomLocation));
                    }
                }
            }

            public override bool IsFirstChapter
            {
                get { return current.Previous == null; }
            }

            public override bool IsLastChapter
            {
                get { return current.Next == null; }
            }

            public override void GoToPreviousChapter()
            {
                if (IsFirstChapter)
                {
                    Log.W(tag, "It's the first chapter already");
                    return;
                }

                current = current.Previous;
                engine.SetChapter(current.Url,
                    InitialLocation.GetLastLocation());
            }

            public override void GoToNextChapter()
            {
                if (IsLastChapter)
                {
                    Log.W(tag, "It's the last chapter already");
                    return;
                }

                current = current.Next;
                engine.SetChapter(current.Url,
                    InitialLocation.GetFirstLocation());
            }
        }
    }
}
