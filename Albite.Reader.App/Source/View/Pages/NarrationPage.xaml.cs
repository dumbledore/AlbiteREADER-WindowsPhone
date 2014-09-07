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

namespace Albite.Reader.App.View.Pages
{
    public partial class NarrationPage : PhoneApplicationPage
    {
        private static readonly string tag = "NarrationPage";

        private XhtmlNarrator narrator;

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
            loadBookPresenter();
            startReading();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //TODO

            // Update the BookLocation of BookPresenter to match
            // the current BookLocation
            //bookPresenter.HistoryStack.SetCurrentLocation(ReaderControl.BookLocation);

            // Now persist BookPresenter
            //bookPresenter.Persist();

            // Stop reading
            narrator.Stop();

            // Going away. No need to keep the resources
            if (e.NavigationMode == NavigationMode.Back)
            {
                unloadNarrator();
            }

            base.OnNavigatedFrom(e);
        }

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

        private void loadNarrator()
        {
            if (narrator == null)
            {
                BookPresenter presenter = App.Context.BookPresenter;

                // Now get the latest location
                BookLocation location = presenter.HistoryStack.GetCurrentLocation();

                // Get the name of the chapter file
                string path = Path.Combine(presenter.ContentPath, location.Chapter.Url);

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
                        XhtmlLocation xhtmlLocation = toXhtmlLocation((DomLocation)location.Location);
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

        private XhtmlLocation toXhtmlLocation(DomLocation location)
        {
            // First we need to take into account the fact that
            // the client JS would add an extra element to the path
            // (because of the root element div, i.e. albite_root),
            // so all paths would start with 0.
            int[] domPath = new int[location.ElementPath.Count - 1];
            Array.Copy(location.ElementPath.ToArray(), 1, domPath, 0, domPath.Length);

            // Also, we need to take into account that the first element after that
            // is offset by +1 (because of albite_start)
            domPath[0] = domPath[0] - 1;

            return new XhtmlLocation(domPath);
        }

        private DomLocation toDomLocation(XhtmlLocation location) {
            int[] oldPath = location.ElementPath.ToArray();
            int[] newPath = new int[oldPath.Length + 1];

            // Take into account that we need to add the rootElement
            Array.Copy(oldPath, 0, newPath, 1, oldPath.Length);
            newPath[0] = 0;

            // And we need to account for the albite_start
            newPath[1] = newPath[1] + 1;

            // TODO: relativeLocation is not calculated here
            return new DomLocation(newPath, 0, 0);
        }

        private void unloadNarrator()
        {
            if (narrator != null)
            {
                narrator.Stop();
                narrator.Dispose();
                narrator = null;
            }
        }

        private void startReading()
        {
            if (narrator != null)
            {
                //Albite.Reader.Core.Diagnostics.Log.I("", narrator.ReadAsync().Status.ToString());
                //Windows.Foundation.IAsyncAction a = narrator.ReadAsync();
                narrator.ReadAsync().Completed = readingCompleted;
            }
        }

        private void stopReading()
        {
            if (narrator != null)
            {
                narrator.Stop();
            }
        }

        private void readingCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
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