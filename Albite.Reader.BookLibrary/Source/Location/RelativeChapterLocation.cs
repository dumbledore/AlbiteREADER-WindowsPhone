using System.Runtime.Serialization;

namespace Albite.Reader.BookLibrary.Location
{
    [DataContract(Name = "relativeChapterLocation")]
    public class RelativeChapterLocation : ChapterLocation
    {
        [DataMember(Name = "relativeLocation")]
        private double relativeLocation { get; set; }

        public RelativeChapterLocation(double relativeLocation)
        {
            this.relativeLocation = relativeLocation;
        }

        public override double RelativeLocation
        {
            get { return relativeLocation; }
        }
    }
}
