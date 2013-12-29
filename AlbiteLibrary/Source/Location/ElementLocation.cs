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

        public override double RelativeLocation
        {
            // We don't know, so simply return 0
            get { return 0; }
        }
    }
}
