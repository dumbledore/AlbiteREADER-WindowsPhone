using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.READER;
using SvetlinAnkov.Albite.READER.Model;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.Tests.Test.View
{
    public class ReaderPageTest : LibraryTest
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
            AlbiteContext context = ((IAlbiteApplication) App.Current).CurrentContext;

            // Get the library
            Library library = context.Library;

            // Add the book
            AddBook(library, book);

            // Navigate
            navigation.Navigate(new Uri("/AlbiteREADER;component/View/ReaderPage.xaml?id=1", UriKind.Relative));
        }
    }
}
