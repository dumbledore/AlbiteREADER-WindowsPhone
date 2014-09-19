using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using System.Windows.Navigation;
using Albite.Reader.Speech.Narration.Xhtml;
using Albite.Reader.BookLibrary.Location;
using System.IO;
using Albite.Reader.Core.IO;
using Albite.Reader.Speech.Narration;
using Albite.Reader.Speech;
using System.Linq;
using System;
using Albite.Reader.Core.Diagnostics;
using System.Globalization;
using Windows.Foundation;
using Microsoft.Phone.Shell;

namespace Albite.Reader.App.View.Pages
{
    public partial class NarrationPage : PhoneApplicationPage
    {
        private static readonly string tag = "NarrationPage";

        private XhtmlNarrator narrator;
        private Chapter chapter;
        private object myLock = new object();

        public NarrationPage()
        {
            InitializeComponent();

            // Get the book presenter
            BookPresenter bookPresenter = App.Context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            lock (myLock)
            {
                loadBookPresenter();
                startReading();
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            lock (myLock)
            {
                // stop the narrator
                stopReading();

                if (chapter != null)
                {
                    // Update the BookLocation of BookPresenter to match
                    // the current BookLocation
                    ILocatedText<XhtmlLocation> current = narrator.LocatedTextManager.Current;
                    if (current != null)
                    {
                        // Relative position is dummy (0), but that shouldn't be an issue as it
                        // is not going to be used anyway, yet it will be updated in ReaderPage
                        persistLocaiton(new DomLocation(current.Location.ElementPath, 0, 0));
                    }

                    // Do not leak the Chapter/BookPresenter
                    chapter = null;
                }

                // Going away. No need to keep the resources
                if (e.NavigationMode == NavigationMode.Back)
                {
                    unloadNarrator();
                }
            }

            base.OnNavigatedFrom(e);
        }

        // shall be called locked
        private void persistLocaiton(ChapterLocation cLocation)
        {
            // Create the book location
            BookLocation bLocation = chapter.CreateLocation(cLocation);

            // Update the location in HistoryStack
            chapter.BookPresenter.HistoryStack.SetCurrentLocation(bLocation);

            // And finally persist
            chapter.BookPresenter.Persist();
        }

        // shall be called locked
        private void loadBookPresenter()
        {
            if (narrator == null)
            {
                // Either page opened for the first time,
                // or app has been tombstoned. In either
                // case we need to load the narrator.

                // First get the BookPresenter
                if (NavigationContext.QueryString.ContainsKey("id"))
                {
                    int bookId = int.Parse(NavigationContext.QueryString["id"]);
                    App.Context.OpenBook(bookId);
                }

                loadNarrator();
            }
        }

        // shall be called locked
        private void loadNarrator()
        {
            if (narrator == null)
            {
                BookPresenter presenter = App.Context.BookPresenter;

                // Now get the latest location
                BookLocation location = presenter.HistoryStack.GetCurrentLocation();

                // Cache the current chapter
                chapter = location.Chapter;

                // Get the name of the chapter file
                string path = Path.Combine(presenter.ContentPath, chapter.Url);

                using (IsolatedStorage iso = new IsolatedStorage(path))
                {
                    using (Stream stream = iso.GetStream(FileAccess.Read))
                    {
                        narrator = new XhtmlNarrator(stream, CultureInfo.CurrentCulture.Name, new NarrationSettings());
                        narrator.LocatedTextManager.TextReached += LocatedTextManager_TextReached;
                    }
                }

                // Now use the provided location
                try
                {
                    if (location.Location is LastPageLocation)
                    {
                        // go to last text location
                    }
                    else if (location.Location is DomLocation)
                    {
                        // go to html
                        DomLocation domLocation = (DomLocation)location.Location;
                        XhtmlLocation xhtmlLocation = new XhtmlLocation(domLocation.ElementPath);
                        ILocatedText<XhtmlLocation> locatedText = narrator.LocatedTextManager[xhtmlLocation];
                        narrator.LocatedTextManager.Current = locatedText;
                    } // all other default to first page - nothing to do
                }
                catch (Exception ex)
                {
                    Log.W(tag, "Failed skipping to the location", ex);
                }
                NarrationBlock.Text = narrator.LocatedTextManager.Current.Text;
            }
        }

        // shall be called locked
        private void unloadNarrator()
        {
            if (narrator != null)
            {
                narrator.Stop();
                narrator.Dispose();
                narrator = null;
            }

            // Do not leak Chapter/BookPresenter
            chapter = null;
        }

        // shall be called locked
        private void startReading()
        {
            if (narrator != null)
            {
                narrator.ReadAsync().Completed = readingCompleted;

                // Disable idle detection so that it can run under locked screen
                PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // shall be called locked
        private void stopReading()
        {
            if (narrator != null)
            {
                narrator.Stop();
            }
        }

        private void readingCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                // Chapter ended.
                // Queue this for later or narrator.Dispose() would dead-lock
                Dispatcher.BeginInvoke(() =>
                    {
                        lock (myLock)
                        {
                            // Just in case
                            stopReading();

                            // Chapter shan't be null, but just in case.
                            // Also, check if it's the last chapter.
                            if (chapter != null && chapter.Next != null)
                            {
                                // Set to next chapter
                                chapter = chapter.Next;

                                // And now persist to first page
                                persistLocaiton(new FirstPageLocation());
                            }

                            // Current narrator is not needed anymore
                            unloadNarrator();

                            // Reload new narrator (for the new location)
                            loadNarrator();

                            // And start
                            startReading();
                        }
                    });
            }
        }

        private void LocatedTextManager_TextReached(LocatedTextManager<XhtmlLocation> sender, ILocatedText<XhtmlLocation> args)
        {
            NarrationBlock.Dispatcher.BeginInvoke(() =>
            {
                NarrationBlock.Text = args.Text;
            });
        }
    }
}