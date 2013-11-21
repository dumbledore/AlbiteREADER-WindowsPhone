using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        public ReaderPage()
        {
            InitializeComponent();

            ReaderControl.Observer = new Observer(this);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) { }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ReaderControl.PersistBook();
            base.OnNavigatingFrom(e);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the book id from the query string
            int bookId = int.Parse(NavigationContext.QueryString["id"]);

            // Now, open the book in the control
            ReaderControl.OpenBook(bookId);
        }

        private class Observer : IReaderControlObserver
        {
            private ReaderPage page;

            public Observer(ReaderPage page)
            {
                this.page = page;
            }

            public void OnError(string message)
            {
                MessageBox.Show("There was a problem with this book:\n" + message);

                // There MUST be a previous entry, otherwise it wouldn't
                // make sense
                page.NavigationService.GoBack();
            }

            public void OnContentLoadingStarted()
            {
                page.WaitControl.Start();
            }

            public void OnContentLoadingCompleted()
            {
                page.WaitControl.Finish();
            }
        }
    }
}