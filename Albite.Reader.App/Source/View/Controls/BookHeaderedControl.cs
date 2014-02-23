using Albite.Reader.BookLibrary;
using System.Windows;

namespace Albite.Reader.App.View.Controls
{
    public class BookHeaderedControl : HeaderedContentControl
    {
        public static readonly DependencyProperty BookProperty
            = DependencyProperty.Register("Bookmark", typeof(Book), typeof(BookHeaderedControl),
            new PropertyMetadata(onBookChanged));

        public Book Book
        {
            get { return (Book)GetValue(BookProperty); }
            set { SetValue(BookProperty, value); }
        }

        private static void onBookChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BookHeaderedControl control = (BookHeaderedControl)d;

            Book book = (Book)e.NewValue;

            // Set title
            control.HeaderText = book.Title;

            // Set author
            control.ContentText = book.Author;
        }
    }
}
