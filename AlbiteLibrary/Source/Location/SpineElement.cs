namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class SpineElement
    {
        public Book Book { get; private set; }
        public int Number { get; private set; }
        public string Url { get; private set; }
        public SpineElement Previous { get; private set; }
        public SpineElement Next { get; private set; }

        internal SpineElement(
            Book book,
            int number,
            string url,
            SpineElement previous,
            SpineElement next = null)
        {
            Book = book;
            Number = number;
            Url = url;
            Previous = previous;
            Next = next;

            if (previous != null)
            {
                previous.Next = this;
            }

            if (next != null)
            {
                next.Previous = this;
            }
        }

        public BookLocation CreateLocation(DomLocation domLocation)
        {
            BookLocation location = new BookLocation(this, domLocation);
            return location;
        }
    }
}
