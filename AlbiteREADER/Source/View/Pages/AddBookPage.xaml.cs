using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Container.Epub;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Navigation;
using Windows.Foundation;
using Windows.Phone.Storage.SharedAccess;
using Windows.Storage;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class AddBookPage : PhoneApplicationPage
    {
        public AddBookPage()
        {
            InitializeComponent();
            initializeWorker();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnBackKeyPress(e);
        }

        private BackgroundWorker worker = new BackgroundWorker();

        private Book book;

        private void initializeWorker()
        {
            // Set up the background worker
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
        }

        private static readonly string newBookFilename = "incoming.epub";

        private void addBook(BackgroundWorker worker, DoWorkEventArgs e)
        {
            Library library = App.Context.Library;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // copy the ebook to temp storage
            IAsyncOperation<IStorageFile> op =
            SharedStorageAccessManager.CopySharedFileAsync(
                ApplicationData.Current.LocalFolder,
                newBookFilename,
                NameCollisionOption.ReplaceExisting,
                App.Context.FileToken);

            while (op.Status != AsyncStatus.Completed)
            {
                Thread.Sleep(100);
            }

            // now install it
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(newBookFilename))
            {
                using (Stream inputStream = iso.GetStream(FileAccess.Read))
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        using (EpubContainer epub = new EpubContainer(zip))
                        {
                            // Add to the library
                            book = library.Books.Add(epub);
                        }
                    }
                }

                // finally, delete it
                iso.Delete();
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            // add the book
            addBook(worker, e);
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // TODO
            }
            else if (e.Error != null)
            {
                // TODO
            }

            // remove it from the context as well
            App.Context.FileToken = null;

            // Navigate directly to the book
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/ReaderPage.xaml?id=" + book.Id, UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WaitControl.Start();
            worker.RunWorkerAsync();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            WaitControl.Finish();
            base.OnNavigatingFrom(e);
        }
    }
}