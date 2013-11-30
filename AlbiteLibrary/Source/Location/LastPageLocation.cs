using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "lastPageLocation")]
    public class LastPageLocation : ChapterLocation
    {
        [DataMember(Name = "lastPage")]
        private bool lastPage = true;

        public override int CompareTo(ChapterLocation other)
        {
            return 1;
        }
    }
}
