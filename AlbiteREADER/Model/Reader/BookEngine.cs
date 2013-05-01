using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public class BookEngine : BrowserEngine
    {
        public BookEngine(IEngineController controller, Settings settings)
            : base(controller, settings) { }

        // TODO: Add history stack

        Book.SpineElement current;

        /// <summary>
        /// Setting this property would cause the engine
        /// to load into the specified book loacation
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
                current = value.SpineElement.Next;
                Chapter = value.SpineElement.Next.Chapter;
                DomLocation = value.DomLocation;
            }
        }

        // Private API
    }
}
