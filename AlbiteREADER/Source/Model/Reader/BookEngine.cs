using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    internal class BookEngine : BrowserEngine
    {
        private static readonly string tag = "BookEngine";

        public BookEngine(IEngineController controller, Settings settings)
            : base(controller, settings) { }

        // TODO: Add history stack

        Book.SpineElement current;

        /// <summary>
        /// Setting this property would cause the engine
        /// to load into the specified book location
        /// </summary>
        public Book.BookLocation BookLocation
        {
            get
            {
                // TODO: What about when the current spine element
                // is not the current chapter, and thus is not
                return null;
            }

            set
            {
                current = value.SpineElement;
                SetChapterDomLocation(value.SpineElement.Url, value.DomLocation);
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
            SetChapterLastPage(current.Url);
        }

        public override void GoToNextChapter()
        {
            if (IsLastChapter)
            {
                Log.W(tag, "It's the last chapter already");
                return;
            }

            current = current.Next;
            SetChapterFirstPage(current.Url);
        }

        // Private API
    }
}
