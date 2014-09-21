using Albite.Reader.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace Albite.Reader.BookLibrary.Location
{
    [DataContract(Name = "chapterLocation")]
    public abstract class ChapterLocation
        : IComparable<ChapterLocation>
    {
        /// <summary>
        /// Relative location in the chapter.
        /// It's a double number in the range 0 (first page) to 1 (last page).
        /// It's not meant to be accurate, rather give some indication
        /// of current progress.
        /// </summary>
        public abstract double RelativeLocation { get; }

        // Default implementation uses relative location
        public virtual int CompareTo(ChapterLocation other)
        {
            return this.RelativeLocation < other.RelativeLocation ? -1 : 1;
        }

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
                ISerializer<object> serializer = LocationSerializer.CreateSerializer();
                return (ChapterLocation)serializer.Decode(encodedData);
            }
            catch (Exception)
            {
                return Default;
            }
        }

        public override string ToString()
        {
            ISerializer<object> serializer = LocationSerializer.CreateSerializer();
            return serializer.Encode(this);
        }
    }
}
