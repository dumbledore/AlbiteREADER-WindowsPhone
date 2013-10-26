using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SvetlinAnkov.Albite.READER;
using SvetlinAnkov.Albite.READER.Model;
using System.ComponentModel;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Container;

namespace SvetlinAnkov.Albite.Tests
{
    public partial class BooksTestPage : PhoneApplicationPage
    {
        private BackgroundWorker worker = new BackgroundWorker();

        private string[] bookPaths = new string[]
        {
            "Test/epub/aliceDynamic.epub",
        };

        private enum WorkType
        {
            WORK_ADD,
            WORK_REMOVE,
        }

        public BooksTestPage()
        {
            InitializeComponent();
            initializeWorker();
        }

        private AlbiteContext getContext()
        {
            return ((IAlbiteApplication)App.Current).CurrentContext;
        }

        private void initializeWorker()
        {
            // Set up the background worker
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
        }

        private void refreshBookList()
        {
            // Remove all buttons
            ContentPanel.Children.Clear();

            BookLibrary.Library library = getContext().Library;

            // Add all books
            IList<Book> books = library.Books.GetAll();
            foreach (Book book in books)
            {
                BookButton button = new BookButton(book.Id);
                button.Content = book.Id + ": " + book.Title;
                button.Click += new RoutedEventHandler(BookButton_Click);
                ContentPanel.Children.Add(button);
            }
        }

        private void addBooks(BackgroundWorker worker, DoWorkEventArgs e)
        {
            Library library = getContext().Library;

            int step = 100 / bookPaths.Length;
            int progress = 0;

            foreach (string bookPath in bookPaths)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                library.Books.Add(new Book.Descriptor(bookPath, BookContainerType.Epub));
                progress += step;
                worker.ReportProgress(progress);
            }
        }

        private void removeBooks(BackgroundWorker worker, DoWorkEventArgs e)
        {
            Library library = getContext().Library;

            IList<Book> books = library.Books.GetAll();

            int step = 100 / books.Count();
            int progress = 0;

            foreach (Book book in books)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                library.Books.Remove(book);
                progress += step;
                worker.ReportProgress(progress);
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            WorkType workType = (WorkType)e.Argument;
            switch (workType)
            {
                case WorkType.WORK_ADD:
                    addBooks(worker, e);
                    break;

                case WorkType.WORK_REMOVE:
                    removeBooks(worker, e);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WaitControl.Progress = e.ProgressPercentage;
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("The operation was cancelled");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("An error has occurred: " + e.Error.Message);
            }

            refreshBookList();
            WaitControl.Finish();
            ApplicationBar.IsVisible = true;
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            BookButton button = (BookButton)sender;

            // Navigate
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/ReaderPage.xaml?id=" + button.Id, UriKind.Relative));
        }

        private void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            refreshBookList();
        }

        private void ApplicationBarIconButton_AddBooks(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("Still working...");
                return;
            }

            ApplicationBar.IsVisible = false;
            WaitControl.Start();
            worker.RunWorkerAsync(WorkType.WORK_ADD);
        }

        private void ApplicationBarIconButton_RemoveBooks(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("Still working...");
                return;
            }

            ApplicationBar.IsVisible = false;
            WaitControl.Start();
            worker.RunWorkerAsync(WorkType.WORK_REMOVE);
        }

        private void ApplicationBarIconButton_RefreshList(object sender, EventArgs e)
        {
            refreshBookList();
        }

        private class BookButton : Button
        {
            public BookButton(int id)
            {
                Id = id;
            }

            public int Id { get; private set; }
        }
    }
}