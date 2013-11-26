namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class Chapter
    {
        public BookPresenter BookPresenter { get; private set; }
        public int Number { get; private set; }
        public string Url { get; private set; }
        public Chapter Previous { get; private set; }
        public Chapter Next { get; private set; }

        internal Chapter(
            BookPresenter bookPresenter,
            int number,
            string url,
            Chapter previous,
            Chapter next = null)
        {
            BookPresenter = bookPresenter;
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
            // No need to call Attach() as creating it like this does that already
            return location;
        }
    }
}
