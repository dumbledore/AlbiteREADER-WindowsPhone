using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Albite.Reader.BookLibrary;
using Albite.Reader.App.View.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Reflection;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BooksPage : PhoneApplicationPage
    {
        private ObservableCollection<Book> books;

        public BooksPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        #region ApplicationBar
        private void InitializeApplicationBar()
        {
            // The buttons can't be addressed using "x:Name", see this:
            // http://stackoverflow.com/questions/5933109/applicationbar-is-always-null
            //
            // So we need to do it the ugly way...

            // First, the icon buttons
            RateButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            AboutButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;

            // Then the menu buttons
        }

        private async void RateButton_Click(object sender, EventArgs e)
        {
            await ExternalLauncher.LaunchAppRatePage();
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            AssemblyName name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            string message = String.Format(
                "Albite READER ver. {0} by Svetlin Ankov\n\n" +
                "Contact me at svetlin.ankov@live.com\n\n" +
                "© 2016 Svetlin Ankov\n", name.Version);

            MessageBox.Show(message, "Albite READER", MessageBoxButton.OK);
        }
        #endregion

        private void setCurrentState()
        {
            // Get the context
            Context context = ((IApplication)App.Current).CurrentContext;

            // Get the book library
            Library library = context.Library;

            // Get the books
            Book[] booksArray = library.Books.GetAll();

            // Fill the observable list
            books = new ObservableCollection<Book>(booksArray);

            // Fill the books
            BooksList.ItemsSource = books;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Go on as usual
            base.OnNavigatedTo(e);

            // Remove previous journal entries (e.g. AddBookPage, etc.)
            while (NavigationService.RemoveBackEntry() != null) { }

            if (e.NavigationMode == NavigationMode.New &&
                NavigationContext.QueryString.ContainsKey("id"))
            {
                // Pass through directly to ReaderPage
                NavigationService.Navigate(
                    new Uri("/Albite.Reader.App;component/Source/View/Pages/ReaderPage.xaml?id=" +
                        NavigationContext.QueryString["id"], UriKind.Relative));
            }
            else
            {
                // Set the current state
                setCurrentState();
            }
        }


        private void BookHeaderedControl_Tap(object sender, GEArgs e)
        {
            BookHeaderedControl control = (BookHeaderedControl)sender;

            // Get the context
            Context context = ((IApplication)App.Current).CurrentContext;

            // Get the library
            Library library = context.Library;

            NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/ReaderPage.xaml?id=" + control.Book.Id, UriKind.Relative));
        }

        private void RemoveBook_Click(object sender, EventArgs e)
        {
            // The sender is actually the menu item
            MenuItem item = (MenuItem)sender;

            // The book
            Book book = (Book)item.CommandParameter;

            string message = string.Format("Are you sure you want to remove \"{0}\" by {1}?", book.Title, book.Author);

            if (MessageBox.Show(message, "Remove book", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Get the context
                Context context = ((IApplication)App.Current).CurrentContext;

                // Get the library
                Library library = context.Library;

                // Unpin
                TileManager.UnpinBook(book);

                // Remove the book
                library.Books.Remove(book);

                // Remove from the observable collection and
                // therefore from the ListBox
                books.Remove(book);
            }
        }
    }
}