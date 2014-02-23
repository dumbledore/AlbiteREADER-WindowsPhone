using Albite.Reader.BookLibrary.Test;
using Albite.Reader.Core.Test;
using Albite.Reader.App;
using System;
using System.Windows.Navigation;

namespace Albite.Reader.Tests
{
    public class ReaderPageTest : TestCase
    {
        private string book;
        private NavigationService navigation;

        public ReaderPageTest(string book, NavigationService navigation)
        {
            this.book = book;
            this.navigation = navigation;
        }

        protected override void TestImplementation()
        {
            // Get the context
            Context context = ((IApplication) App.Current).CurrentContext;

            // Get the library
            BookLibrary.Library library = context.Library;

            // Add the book
            LibraryHelper.AddEpubFromResource(book, library);

            // Navigate
            navigation.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/ReaderPage.xaml?id=1", UriKind.Relative));
        }
    }
}
