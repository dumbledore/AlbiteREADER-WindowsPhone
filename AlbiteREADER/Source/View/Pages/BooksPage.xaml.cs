using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class BooksPage : PhoneApplicationPage
    {
        public BooksPage()
        {
            InitializeComponent();
        }

        private void setCurrentState()
        {
            // Clear the contents
            BookList.Children.Clear();

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book library
            Library library = context.Library;

            // Get the books
            Book[] books = library.Books.GetAll();

            // Fill with books
            foreach (Book book in books)
            {
                // Create the control
                MyControl control = new MyControl(book);

                // Enable tilt effect
                control.SetValue(TiltEffect.IsTiltEnabledProperty, true);

                // Attach the context menu
                attachContextMenu(control);

                // Add handler
                control.Tap += control_Tap;

                // Add to the other controls
                BookList.Children.Add(control);
            }
        }

        private static void attachContextMenu(DependencyObject control)
        {
            // Create the menu item
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Remove";
            menuItem.Command = new RemoveBookCommand();
            menuItem.CommandParameter = control;

            // Create the context menu
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem);

            // Add the context menu
            control.SetValue(ContextMenuService.ContextMenuProperty, contextMenu);
        }

        private void control_Tap(object sender, GEArgs e)
        {
            MyControl control = (MyControl)sender;

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the library
            Library library = context.Library;

            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/ReaderPage.xaml?id=" + control.Book.Id, UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

        #region MyControl
        private class MyControl : HeaderedContentControl
        {
            public MyControl(Book book)
            {
                // Set title
                HeaderText = book.Title;

                // Set author
                ContentText = book.Author;

                // Set book
                Book = book;
            }

            public Book Book { get; private set; }
        }
        #endregion

        #region RemoveBookCommand
        private class RemoveBookCommand : ICommand
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
                MyControl control = (MyControl)parameter;

                string message = string.Format("Are you sure you want to remove \"{0}\" by {1}?", control.Book.Title, control.Book.Author);

                if (MessageBox.Show(message, "Remove book", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    // Get the context
                    AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

                    // Get the library
                    Library library = context.Library;

                    // Unpin
                    TileManager.UnpinBook(control.Book);

                    // Remove the book
                    library.Books.Remove(control.Book);

                    // Get the parent
                    Panel parent = (Panel)control.Parent;

                    // Remove the control
                    parent.Children.Remove(control);
                }
            }
        }
        #endregion
    }
}