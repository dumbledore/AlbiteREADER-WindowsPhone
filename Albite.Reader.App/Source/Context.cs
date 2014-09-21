using Microsoft.Phone.Shell;
using Albite.Reader.BookLibrary;
using Albite.Reader.Core.Serialization;
using Albite.Reader.Engine.Layout;
using Albite.Reader.App.View.Pages.BookSettings;
using System.Collections.Generic;
using System.IO;
using Albite.Reader.Speech.Narration;

namespace Albite.Reader.App
{
    public class Context
    {
        public Library Library { get; private set; }
        public RecordStore RecordStore { get; private set; }

        public Context(string libraryPath, string recordStorePath)
        {
            Library = new BookLibrary.Library(libraryPath);
            RecordStore = new RecordStore(recordStorePath);
        }

        public static readonly string LayoutSettingsKey = "layout-settings";

        private LayoutSettings cachedSettings = null;
        public LayoutSettings LayoutSettings
        {
            get
            {
                if (cachedSettings == null)
                {
                    if (RecordStore.ContainsKey(LayoutSettingsKey))
                    {
                        // Get data
                        string s = RecordStore[LayoutSettingsKey];

                        // Unserialize
                        cachedSettings = LayoutSettings.FromString(s);
                    }
                    else
                    {
                        // Default Settings
                        cachedSettings = DefaultLayoutSettings.LayoutSettings;

                        // No need to persist them as
                        // they would be created again
                        // next time if they are not changed
                        // and therefore not persisted
                    }
                }

                return cachedSettings;
            }

            set
            {
                // Cache
                cachedSettings = value;

                // Persist
                RecordStore[LayoutSettingsKey] = value.ToString();
            }
        }

        public static readonly string NarrationSettingsKey = "narration-settings";

        private NarrationSettings cachedNarrationSettings = null;
        public NarrationSettings NarrationSettings
        {
            get
            {
                if (cachedNarrationSettings == null)
                {
                    if (RecordStore.ContainsKey(NarrationSettingsKey))
                    {
                        // Get data
                        string s = RecordStore[NarrationSettingsKey];

                        // Unserialize
                        cachedNarrationSettings = NarrationSettings.FromString(s);
                    }
                    else
                    {
                        // Default Settings
                        cachedNarrationSettings = new NarrationSettings();
                    }
                }

                return cachedNarrationSettings;
            }

            set
            {
                // Cache
                cachedNarrationSettings = value;

                // Persist
                RecordStore[NarrationSettingsKey] = value.ToString();
            }
        }


        private static readonly string BookPresenterKey = "book-presenter";

        private BookPresenter bookPresenterCached = null;

        /// <summary>
        /// BookPresenter for the currently opened book
        /// </summary>
        public BookPresenter BookPresenter
        {
            get
            {
                if (bookPresenterCached == null)
                {
                    IDictionary<string, object> state = PhoneApplicationService.Current.State;

                    if (state.ContainsKey(BookPresenterKey))
                    {
                        int bookId = (int)state[BookPresenterKey];
                        bookPresenterCached = openBookInternal(bookId);
                    }
                }

                return bookPresenterCached;
            }
        }

        public BookPresenter OpenBook(int bookId)
        {
            // Open the book only if there's no
            // book opened and if there is, it's
            // not the same book
            if (bookPresenterCached == null
                || bookPresenterCached.Book.Id != bookId)
            {
                // Cache the new book
                bookPresenterCached = openBookInternal(bookId);

                // Get the current state
                IDictionary<string, object> state = PhoneApplicationService.Current.State;

                // And add it to the state
                state[BookPresenterKey] = bookId;
            }

            return bookPresenterCached;
        }

        private BookPresenter openBookInternal(int bookId)
        {
            // Get the book for the given id
            Book book = Library.Books[bookId];

            // return the new book presenter for this book
            return new BookPresenter(book);
        }

        // File token used when adding books
        public string FileToken { get; set; }

        // Stream used when adding books
        public Stream FileStream { get; set; }
    }
}
