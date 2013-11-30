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

        public override int CompareTo(ChapterLocation other)
        {
            if (other is PageLocation)
            {
                PageLocation otherPage = other as PageLocation;
                return this.Page < otherPage.Page ? -1 : 1;
            }
            else if (other is FirstPageLocation)
            {
                return 1;
            }
            else if (other is LastPageLocation)
            {
                return -1;
            }

            // We can't say anything for ElementLocation or DomLocation,
            // so return a default value, e.g. -1
            return -1;
        }
    }
}
