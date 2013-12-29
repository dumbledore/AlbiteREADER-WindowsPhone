using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "pageLocation")]
    public class PageLocation : ChapterLocation
    {
        [DataMember(Name = "page")]
        public int Page { get; private set; }

        public PageLocation(int page)
        {
            Page = page;
        }

        public override double RelativeLocation
        {
            // We don't know, so simply return 0
            get { return 0; }
        }

        public override int CompareTo(ChapterLocation other)
        {
            if (other is PageLocation)
            {
                PageLocation otherPage = other as PageLocation;
                return this.Page < otherPage.Page ? -1 : 1;
            }
            else
            {
                // Use default
                return base.CompareTo(other);
            }
        }
    }
}
