using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        private int bookId;

        public ReaderPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) { }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the book id from the query string
            bookId = int.Parse(NavigationContext.QueryString["id"]);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ReaderControl.PersistBook();
            base.OnNavigatingFrom(e);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReaderControl.OpenBook(bookId);
        }

        private void ReaderControl_ReaderError(object sender, EventArgs e)
        {
            MessageBox.Show("There was a problem with this book");

            // There MUST be a previous entry, otherwise it wouldn't
            // make sense
            NavigationService.GoBack();
        }

        private void ReaderControl_ContentLoadingStarted(object sender, EventArgs e)
        {
            WaitControl.Start();
        }

        private void ReaderControl_ContentLoadingCompleted(object sender, EventArgs e)
        {
            WaitControl.Finish();
        }
    }
}