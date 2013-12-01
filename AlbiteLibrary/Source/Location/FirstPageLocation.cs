using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "firstPageLocation")]
    public class FirstPageLocation : ChapterLocation
    {

#pragma warning disable 0414
        [DataMember(Name="firstPage")]
        private bool firstPage = true;
#pragma warning restore 0414

        public override int CompareTo(ChapterLocation other)
        {
            return -1;
        }
    }
}
