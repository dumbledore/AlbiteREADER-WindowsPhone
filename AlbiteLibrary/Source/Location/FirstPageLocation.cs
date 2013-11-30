using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "firstPageLocation")]
    public class FirstPageLocation : ChapterLocation
    {
        [DataMember(Name="firstPage")]
        private bool firstPage = true;

        public override int CompareTo(ChapterLocation other)
        {
            return -1;
        }
    }
}
