using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "lastPageLocation")]
    public class LastPageLocation : ChapterLocation
    {

#pragma warning disable 0414
        [DataMember(Name = "lastPage")]
        private bool lastPage = true;
#pragma warning restore 0414

        public override int CompareTo(ChapterLocation other)
        {
            return 1;
        }
    }
}
