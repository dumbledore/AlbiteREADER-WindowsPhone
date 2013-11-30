using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "elementLocation")]
    public class ElementLocation : ChapterLocation
    {
        [DataMember(Name = "elementId")]
        public string ElementId { get; private set; }

        public ElementLocation(string elementId)
        {
            ElementId = elementId;
        }

        public override int CompareTo(ChapterLocation other)
        {
            if (other is FirstPageLocation)
            {
                return 1;
            }
            else if (other is LastPageLocation)
            {
                return -1;
            }

            // We can't say anything for ElementLocation,
            // DomLocation or PageLocation,
            // so return a default value, e.g. -1
            return -1;
        }
    }
}
