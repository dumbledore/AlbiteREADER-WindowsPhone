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

namespace Albite.Reader.App.View.Pages
{
    public partial class AddBookPage : PhoneApplicationPage
    {
        public AddBookPage()
        {
            InitializeComponent();
        }

        private CancellationTokenSource cancelSource = new CancellationTokenSource();

        private static readonly string newBookFilename = "incoming.epub";

        private async Task<Book> addBook(CancellationToken cancelToken, IProgress<double> progress)
        {
            string fileToken = App.Context.FileToken;

            if (fileToken == null)
            {
                throw new InvalidOperationException("FileToken is null");
            }

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

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Start loading
            WaitControl.Start();

            try
            {
                // Get book async
                Book book = await addBook(cancelSource.Token, null);

                // Remove file token from the context
                App.Context.FileToken = null;

                // Navigate to the book
                NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/BooksPage.xaml?id=" + book.Id, UriKind.Relative));
            }
            catch (OperationCanceledException)
            {
                // Canceled
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
            catch (Exception ex)
            {
                // Errors
                MessageBox.Show(
                    "An error has occurred while processing the file: " + ex.Message,
                    "Could not add book",
                    MessageBoxButton.OK);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Cancel the task just in case
            cancelSource.Cancel();

            // Finished loading
            WaitControl.Finish();

            base.OnNavigatingFrom(e);
        }
    }
}