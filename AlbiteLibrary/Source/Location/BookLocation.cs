using System;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class BookLocation : IComparable<BookLocation>
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

        public int CompareTo(BookLocation other)
        {
            int thisSpineIndex = SpineElement.Number;
            int otherSpineIndex = other.SpineElement.Number;

            if (thisSpineIndex != otherSpineIndex)
            {
                return thisSpineIndex < otherSpineIndex ? -1 : 1;
            }

            return DomLocation.CompareTo(other.DomLocation);
        }
    }
}
