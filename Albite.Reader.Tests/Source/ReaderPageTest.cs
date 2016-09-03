using Albite.Reader.BookLibrary.Test;
using Albite.Reader.Core.Test;
using Albite.Reader.App;
using System;
using System.Windows.Navigation;
using Albite.Reader.BookLibrary;

namespace Albite.Reader.Tests
{
    public class ReaderPageTest : TestCase
    {
        private string _book;
        private NavigationService _navigation;

        public ReaderPageTest(string book, NavigationService navigation)
        {
            _book = book;
            _navigation = navigation;
        }

        protected override async void TestImplementation()
        {
            // Get the context
            Context context = ((IApplication) App.Current).CurrentContext;

            // Get the library
            BookLibrary.Library library = context.Library;

            // Add the book
            Book book = await LibraryHelper.AddEpubFromResource(_book, library);

            // Navigate
            _navigation.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/ReaderPage.xaml?id=" + book.Id, UriKind.Relative));
        }
    }
}
