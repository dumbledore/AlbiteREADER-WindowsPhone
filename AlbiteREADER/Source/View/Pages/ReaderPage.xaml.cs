using Microsoft.Phone.Controls;
using System;
using System.Windows;

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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the book id from the query string
            bookId = int.Parse(NavigationContext.QueryString["id"]);
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

        // TODO: Add proper thread synchronization
    }
}