using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using Albite.Reader.Container.Epub;
using Albite.Reader.Core.IO;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Windows.Phone.Storage.SharedAccess;
using Windows.Storage;
using Albite.Reader.Core.Threading;

namespace Albite.Reader.App.View.Pages
{
    public partial class AddBookPage : PhoneApplicationPage
    {
        public AddBookPage()
        {
            InitializeComponent();
        }

        private CancellableTask currentTask;

        private static readonly string newBookFilename = "incoming.epub";

        private void cancelCurrentTask()
        {
            if (currentTask != null)
            {
                currentTask.Cancel();
                currentTask = null;
            }

            WaitControl.Finish();
        }

        private void addBook(IProgress<double> progress)
        {
            // Now create the task
            CancellationTokenSource cts = new CancellationTokenSource();
            currentTask = new CancellableTask(addBookAsync(cts.Token, progress), cts);
        }

        private async Task addBookAsync(CancellationToken cancelToken, IProgress<double> progress)
        {

            Stream fileStream = App.Context.FileStream;
            string fileToken = App.Context.FileToken;
            Book book;

            try
            {
                // Get book async
                if (fileStream != null)
                {
                    book = await addBookStreamAsync(fileStream, cancelToken, progress);
                    App.Context.FileStream = null;
                }
                else if (fileToken != null)
                {
                    book = await addBookTokenAsync(fileToken, cancelToken, progress);
                    App.Context.FileToken = null;
                }
                else
                {
                    throw new InvalidOperationException("No book source found");
                }

                // Navigate to the book
                NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/BooksPage.xaml?id=" + book.Id, UriKind.Relative));
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    MessageBox.Show(
                        "Book was not added to library",
                        "Cancelled",
                        MessageBoxButton.OK);
                }
                else
                {
                    // Errors
                    MessageBox.Show(
                        "An error has occurred while processing the file: " + ex.Message,
                        "Could not add book",
                        MessageBoxButton.OK);

                    // Discard the page on error
                    discard();
                }
            }
        }

        private void discard()
        {
            if (NavigationService.CanGoBack)
            {
                // Return to previous page
                NavigationService.GoBack();
            }
            else
            {
                // Terminate the app completely
                Application.Current.Terminate();
            }
        }

        private async Task<Book> addBookStreamAsync(
            Stream fileStream, CancellationToken cancelToken, IProgress<double> progress)
        {
            Library library = App.Context.Library;

            // check if canceled
            cancelToken.ThrowIfCancellationRequested();

            // Set progress
            if (progress != null)
            {
                progress.Report(double.NaN);
            }

            // now install it (this will dispose the stream as well)
            using (Stream inputStream = fileStream)
            {
                using (ZipContainer zip = new ZipContainer(inputStream))
                {
                    using (EpubContainer epub = new EpubContainer(zip))
                    {
                        // check if canceled. this will delete the input e-pub as well
                        cancelToken.ThrowIfCancellationRequested();

                        // Add to the library
                        return await library.Books.AddAsync(epub, cancelToken, progress);
                    }
                }
            }
        }

        private async Task<Book> addBookTokenAsync(string fileToken, CancellationToken cancelToken, IProgress<double> progress)
        {
            Library library = App.Context.Library;

            // check if canceled
            cancelToken.ThrowIfCancellationRequested();

            // Set progress
            if (progress != null) {
                progress.Report(double.NaN);
            }

            // copy the ebook to temp storage
            await SharedStorageAccessManager.CopySharedFileAsync(
                ApplicationData.Current.LocalFolder,
                newBookFilename,
                NameCollisionOption.ReplaceExisting,
                fileToken);

            // now install it
            using (IsolatedStorage iso = new IsolatedStorage(newBookFilename))
            {
                try
                {
                    using (Stream inputStream = iso.GetStream(FileAccess.Read))
                    {
                        using (ZipContainer zip = new ZipContainer(inputStream))
                        {
                            using (EpubContainer epub = new EpubContainer(zip))
                            {
                                // check if canceled. this will delete the input e-pub as well
                                cancelToken.ThrowIfCancellationRequested();

                                // Add to the library
                                return await library.Books.AddAsync(epub, cancelToken, progress);
                            }
                        }
                    }
                }
                finally
                {
                    // delete it in all cases
                    iso.Delete();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If going back, the task must have been cancelled,
            // we are not going to start a new one.
            if (e.NavigationMode == NavigationMode.New)
            {
                // Start loading
                WaitControl.Start();

                // Start async operation
                addBook(new Progress(this));
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Cancel the task just in case
            cancelCurrentTask();

            if (!e.IsNavigationInitiator)
            {
                // Going away from the app.
                // Most likely the home button has been pressed.
                // Discard the page
                discard();
            }

            base.OnNavigatingFrom(e);
        }

        private class Progress : IProgress<double>
        {
            private AddBookPage page;

            public Progress(AddBookPage page)
            {
                this.page = page;
            }

            public void Report(double value)
            {
                page.WaitControl.Dispatcher.BeginInvoke(() =>
                    {
                        if (double.IsNaN(value))
                        {
                            page.WaitControl.IsIndeterminate = true;
                        }
                        else
                        {
                            page.WaitControl.Progress = value;
                            page.WaitControl.IsIndeterminate = false;
                        }
                    });
            }
        }
    }
}