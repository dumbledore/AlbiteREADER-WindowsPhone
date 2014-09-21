using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Core.IO;
using Albite.Reader.Speech;
using Albite.Reader.Speech.Narration;
using Albite.Reader.Speech.Narration.Xhtml;
using Microsoft.Phone.Shell;
using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Windows.Foundation;

namespace Albite.Reader.App
{
    public class NarrationController : IDisposable
    {
        public event TypedEventHandler<NarrationController, string> UpdateText;
        public event EventHandler NarrationEnded;

        public NarrationSettings NarrationSettings { get; private set; }

        private XhtmlNarrator narrator;
        private Chapter chapter;
        private Dispatcher dispatcher;
        private bool disposed = false;
        private object myLock = new object();

        private static readonly string tag = "NarrationController";

        public NarrationController(NarrationSettings settings, Dispatcher dispatcher)
        {
            this.NarrationSettings = settings;
            this.dispatcher = dispatcher;

            // Get the BookPresenter
            BookPresenter presenter = App.Context.BookPresenter;

            // Now get the latest location
            BookLocation location = presenter.HistoryStack.GetCurrentLocation();

            // Now load the narrator. No need to hold a lock as
            // no text is being read, i.e. no events will occurr.
            loadNarrator(location);
        }

        public void Dispose()
        {
            lock (myLock)
            {
                throwIfDisposed();
                unloadNarrator();
                disposed = true;
            }
        }

        public void StartReading()
        {
            lock (myLock)
            {
                throwIfDisposed();
                startReadingLocked();
            }
        }

        public void StopReading()
        {
            lock (myLock)
            {
                throwIfDisposed();
                stopReadingLocked();
            }
        }

        public void StopReadingAndPersist()
        {
            lock (myLock)
            {
                throwIfDisposed();

                // First, stop reading
                stopReadingLocked();

                // Update the BookLocation of BookPresenter to match the current BookLocation
                ILocatedText<XhtmlLocation> current = narrator.LocatedTextManager.Current;
                if (current != null)
                {
                    // Relative position is dummy (0), but that shouldn't be an issue as it
                    // is not going to be used anyway, yet it will be updated in ReaderPage
                    persistLocaiton(new DomLocation(current.Location.ElementPath, 0, 0));
                }
                else
                {
                    // Somehow current is null. Still, had beter persist the chapter at least
                    persistLocaiton(new FirstPageLocation());
                }
            }
        }

        public void GoBack()
        {
            lock (myLock)
            {
                go(true);
            }
        }

        public void GoForward()
        {
            lock (myLock)
            {
                go(false);
            }
        }

        private void go(bool previous)
        {
            stopReadingLocked();

            ILocatedText<XhtmlLocation> loc = getLocation(previous);
            if (loc != null)
            {
                narrator.LocatedTextManager.Current = loc;

                if (UpdateText != null)
                {
                    UpdateText(this, loc.Text);
                }
            }
        }

        private ILocatedText<XhtmlLocation> getLocation(bool previous)
        {
            ILocatedText<XhtmlLocation> loc = null;

            // Trying to find it in the current chapter
            ILocatedText<XhtmlLocation> current = narrator.LocatedTextManager.Current;

            if (current != null)
            {
                // Get the last/first location before/after current that actually contains any text
                loc = previous
                    ? narrator.LocatedTextManager.Locations.LastOrDefault(l => l.Id < current.Id && l.Text.Length > 0)
                    : narrator.LocatedTextManager.Locations.FirstOrDefault(l => l.Id > current.Id && l.Text.Length > 0);
            }

            if (loc == null)
            {
                Chapter c = previous ? chapter.Previous : chapter.Next;

                // Start searching in previous/next chapters
                while(c != null)
                {
                    // Going to previous/next chapter. First unload the narrator
                    unloadNarrator();

                    // Update chapter info
                    chapter = c;

                    // Now load for the chapter we just set
                    loadNarrator();

                    // Get the last/first location that actually contains any text
                    loc = previous
                        ? narrator.LocatedTextManager.Locations.LastOrDefault(l => l.Text.Length > 0)
                        : narrator.LocatedTextManager.Locations.FirstOrDefault(l => l.Text.Length > 0);

                    if (loc != null)
                    {
                        // Found it
                        break;
                    }

                    c = previous ? c.Previous : c.Next;
                }
            }

            return loc;
        }

        private void readingCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                // Chapter ended.
                // Queue this for later or narrator.Dispose() would dead-lock
                dispatcher.BeginInvoke(() =>
                {
                    lock (myLock)
                    {
                        throwIfDisposed();

                        // Just in case
                        stopReadingLocked();

                        // Check if it's the last chapter.
                        if (chapter.Next != null)
                        {
                            goToNextChapter();
                        }
                        else
                        {
                            // Book ended
                            if (NarrationEnded != null)
                            {
                                // No need to call from the dispatcher,
                                // as we are already there!
                                NarrationEnded(this, EventArgs.Empty);
                            }
                        }
                    }
                });
            }
        }

        private void goToPreviousChapter()
        {
            // Set to previous chapter
            chapter = chapter.Previous;

            // And now reload
            reloadChapter(new LastPageLocation());
        }

        private void goToNextChapter()
        {
            // Set to next chapter
            chapter = chapter.Next;

            // And now reload
            reloadChapter(new FirstPageLocation());
        }

        private void reloadChapter(ChapterLocation location)
        {
            // And now persist to first page
            BookLocation bLocation = persistLocaiton(location);

            // Current narrator is not needed anymore
            unloadNarrator();

            // Reload new narrator (for the new location)
            loadNarrator(bLocation);

            // And start
            startReadingLocked();
        }

        private void startReadingLocked()
        {
            narrator.ReadAsync().Completed = readingCompleted;

            // Disable idle detection so that it can run under locked screen
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        private void stopReadingLocked()
        {
            narrator.Stop();
        }

        private void loadNarrator(BookLocation location)
        {
            // Cache the current chapter
            chapter = location.Chapter;

            // Now load the narrator for the provided chapter
            loadNarrator();

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

            if (UpdateText != null)
            {
                UpdateText(this, narrator.LocatedTextManager.Current.Text);
            }
        }

        // This loads the narrator for the current chapter
        private void loadNarrator()
        {
            // Get the name of the chapter file
            string path = Path.Combine(chapter.BookPresenter.ContentPath, chapter.Url);

            using (IsolatedStorage iso = new IsolatedStorage(path))
            {
                using (Stream stream = iso.GetStream(FileAccess.Read))
                {
                    narrator = new XhtmlNarrator(stream, NarrationSettings);
                    narrator.LocatedTextManager.TextReached += textReached;
                }
            }
        }

        private void unloadNarrator()
        {
            narrator.Stop();
            narrator.Dispose();
            narrator = null;

            // Do not leak Chapter/BookPresenter
            chapter = null;
        }

        private BookLocation persistLocaiton(ChapterLocation cLocation)
        {
            // Create the book location
            BookLocation bLocation = chapter.CreateLocation(cLocation);

            // Update the location in HistoryStack
            chapter.BookPresenter.HistoryStack.SetCurrentLocation(bLocation);

            // And finally persist
            chapter.BookPresenter.Persist();

            // Now return the newly created BookLocation
            return bLocation;
        }

        private void textReached(LocatedTextManager<XhtmlLocation> sender, ILocatedText<XhtmlLocation> args)
        {
            if (UpdateText != null)
            {
                UpdateText(this, args.Text);
            }
        }

        private void throwIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
