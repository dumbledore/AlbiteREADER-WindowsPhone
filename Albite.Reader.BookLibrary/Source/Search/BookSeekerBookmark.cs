using Albite.Reader.BookLibrary.Location;

namespace Albite.Reader.BookLibrary.Search
{
    internal class BookSeekerBookmark : IBookmark
    {

        public BookLocation BookLocation { get; private set; }

        public string Text { get; private set; }

        public BookSeekerBookmark(BookLocation bookLocation, string text)
        {
            this.BookLocation = bookLocation;
            this.Text = text;
        }

        public int CompareTo(IBookmark other)
        {
            return BookLocation.CompareTo(other.BookLocation);
        }
    }
}
