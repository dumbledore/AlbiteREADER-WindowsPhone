using System.Runtime.Serialization;

namespace Albite.Reader.BookLibrary.Location
{
    [DataContract(Name = "lastPageLocation")]
    public class LastPageLocation : ChapterLocation
    {

#pragma warning disable 0414
        [DataMember(Name = "lastPage")]
        private bool lastPage = true;
#pragma warning restore 0414

        public override double RelativeLocation
        {
            // Last page, so always 1
            get { return 1; }
        }
    }
}
