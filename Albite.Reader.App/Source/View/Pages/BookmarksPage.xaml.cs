using Microsoft.Phone.Controls;
using Albite.Reader.BookLibrary;
using Albite.Reader.App.View.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BookmarksPage : PhoneApplicationPage
    {
        ObservableCollection<Bookmark> bookmarks;

        public BookmarksPage()
        {
            InitializeComponent();

            // Get the book presenter
            BookPresenter bookPresenter = App.Context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;

            // Get all current bookmakrs
            Bookmark[] bookmarkArray = bookPresenter.BookmarkManager.GetAll();

            // Sort them so that they are listed correctly
            Array.Sort(bookmarkArray);

            // Create an observable collection so that
            // it will be in sync
            bookmarks = new ObservableCollection<Bookmark>(bookmarkArray);

            // Fill the bookmarks
            BookmarksList.ItemsSource = bookmarks;
        }

        private void RemoveBookmark_Click(object sender, EventArgs e)
        {
            // The sender is actually the menu item
            MenuItem item = (MenuItem)sender;

            // The bookmark
            Bookmark bookmark = (Bookmark)item.CommandParameter;

            // Note that we could get the containing control,
            // by taking into account that MenuItem.ParentMenuBase
            // is a ContextMenu, and using ContextMenu.Owner

            if (MessageBox.Show(
                "Are you sure you want to delete this bookmark?",
                "Delete bookmark",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Get the book presenter
                BookPresenter bookPresenter = App.Context.BookPresenter;

                // Remove the bookmark
                bookPresenter.BookmarkManager.Remove(bookmark);

                // Remove from the observable collection and
                // therefore from the ListBox
                bookmarks.Remove(bookmark);
            }
        }

        private void BookmarkControl_Tap(object sender, GEArgs e)
        {
            BookmarkControl control = (BookmarkControl)sender;

            // Get the book presenter
            BookPresenter bookPresenter = App.Context.BookPresenter;

            // Update the reading location
            bookPresenter.HistoryStack.AddNewLocation(control.Bookmark.BookLocation);

            // Go back to ReaderPage
            NavigationService.GoBack();
        }
    }
}