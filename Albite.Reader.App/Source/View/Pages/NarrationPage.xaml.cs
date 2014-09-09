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