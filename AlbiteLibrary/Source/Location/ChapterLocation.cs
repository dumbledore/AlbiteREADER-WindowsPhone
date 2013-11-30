using SvetlinAnkov.Albite.Core.Json;
using System;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "chapterLocation")]
    public abstract class ChapterLocation
        : IComparable<ChapterLocation>
    {
        public abstract int CompareTo(ChapterLocation other);

        public static ChapterLocation Default
        {
            get { return new FirstPageLocation(); }
        }

        public static ChapterLocation FromString(string encodedData)
        {
            if (encodedData == null)
            {
                return Default;
            }

            try
            {
                LocationSerializer serializer = new LocationSerializer();
                return (ChapterLocation)serializer.Decode(encodedData);
            }
            catch (Exception)
            {
                return Default;
            }
        }

        public override string ToString()
        {
            LocationSerializer serializer = new LocationSerializer();
            return serializer.Encode(this);
        }
    }
}
