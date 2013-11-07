namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class BookLocation
    {
        public SpineElement SpineElement { get; private set; }
        public DomLocation DomLocation { get; private set; }

        internal BookLocation(
            SpineElement spineElement,
            DomLocation domLocation)
        {
            SpineElement = spineElement;
            DomLocation = domLocation;
        }
    }
}
