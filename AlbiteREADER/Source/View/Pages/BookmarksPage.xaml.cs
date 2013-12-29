using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class BookmarksPage : PhoneApplicationPage
    {
        public BookmarksPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;

            // Get all current bookmakrs
            Bookmark[] bookmarks = bookPresenter.BookmarkManager.GetAll();

            // Sort them so that they are listed correctly
            Array.Sort(bookmarks);

            // Fill the bookmarks
            foreach (Bookmark bookmark in bookmarks)
            {
                // Create the control
                BookmarkControl control = new BookmarkControl(bookmark);

                // Attach the context menu
                attachContextMenu(control);

                // Attach the tap event handler
                control.Tap += bookmarkControl_Tap;

                // Add to the other controls
                BookmarksList.Children.Add(control);
            }
        }

        private static void attachContextMenu(DependencyObject control)
        {
            // Create the menu item
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Remove";
            menuItem.Command = new RemoveBookmarkCommand();
            menuItem.CommandParameter = control;

            // Create the context menu
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem);

            // Add the context menu
            control.SetValue(ContextMenuService.ContextMenuProperty, contextMenu);
        }

        private void bookmarkControl_Tap(object sender, GEArgs e)
        {
            BookmarkControl control = (BookmarkControl)sender;

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Update the reading location
            bookPresenter.BookLocation = control.Bookmark.BookLocation;

            // TODO: Add to history stack

            // Go back to ReaderPage
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        #region BookmarkControl
        private class BookmarkControl : HeaderedContentControl
        {
            public Bookmark Bookmark { get; private set; }

            public BookmarkControl(Bookmark bookmark)
            {
                Bookmark = bookmark;

                // Set up book position as header text
                HeaderText = string.Format(
                    "{0:P0}", getReadingPosition(bookmark));

                // Set up bookmark text as content text
                ContentText = bookmark.Text;
            }

            private static double getReadingPosition(Bookmark bookmark)
            {
                BookLocation location = bookmark.BookLocation;

                // Total number of chapters
                int chapterCount = location.Chapter.BookPresenter.Spine.Length;

                // Each chapter gets this interval in [0, 1]
                double chapterInterval = 1 / (double)chapterCount;

                // Chapter starts at this position
                double chapterPosition = location.Chapter.Number * chapterInterval;

                // Additional offset to account for chapter location
                double chapterOffset = location.Location.RelativeLocation * chapterInterval;

                // return the total
                return chapterPosition + chapterOffset;
            }
        }
        #endregion

        #region RemoveBookmarkCommand
        private class RemoveBookmarkCommand : ICommand
        {
#pragma warning disable 0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                BookmarkControl control = (BookmarkControl)parameter;

                // Get the context
                AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

                // Get the book presenter
                BookPresenter bookPresenter = context.BookPresenter;

                // Remove the bookmark
                bookPresenter.BookmarkManager.Remove(control.Bookmark);

                // Get the parent
                Panel parent = (Panel)control.Parent;

                // Remove the control
                parent.Children.Remove(control);
            }
        }
        #endregion
    }
}