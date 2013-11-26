using System;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class BookLocation : IComparable<BookLocation>
    {
        public Chapter Chapter { get; private set; }
        public DomLocation DomLocation { get; private set; }

        internal BookLocation(
            Chapter chapter,
            DomLocation domLocation)
        {
            Chapter = chapter;
            DomLocation = domLocation;
        }

        public int CompareTo(BookLocation other)
        {
            int thisSpineIndex = Chapter.Number;
            int otherSpineIndex = other.Chapter.Number;

            if (thisSpineIndex != otherSpineIndex)
            {
                return thisSpineIndex < otherSpineIndex ? -1 : 1;
            }

            return DomLocation.CompareTo(other.DomLocation);
        }
    }
}
