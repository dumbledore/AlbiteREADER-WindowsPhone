using System.Runtime.Serialization;

namespace Albite.Reader.BookLibrary.Location
{
    [DataContract(Name = "firstPageLocation")]
    public class FirstPageLocation : ChapterLocation
    {

#pragma warning disable 0414
        [DataMember(Name="firstPage")]
        private bool firstPage = true;
#pragma warning restore 0414

        public override double RelativeLocation
        {
            // First page, so always 0
            get { return 0; }
        }
    }
}
