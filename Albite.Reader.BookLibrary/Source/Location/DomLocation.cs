using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Albite.Reader.BookLibrary.Location
{
    [DataContract(Name = "domLocation")]
    public class DomLocation : ChapterLocation
    {
        [DataMember(Name = "elementPath")]
        private int[] elementPath;
        public IList<int> ElementPath
        {
            get { return Array.AsReadOnly<int>(elementPath); }
        }

        [DataMember(Name = "textOffset")]
        public int TextOffset { get; private set; }

        [DataMember(Name = "relativeLocation")]
        private double relativeLocation { get; set; }

#pragma warning disable 0414
        // Versioning for backwards compatibility
        //
        // Default to 0, i.e. old locaiton.
        // If unparsing an old location, it will
        // be kept to the default, i.e zero.
        //
        // The JSClient will return new locations
        // with a version > 0, but will be able
        // to handle old ones as well.
        [DataMember(Name = "version")]
        private int version = 0;
#pragma warning restore 0414

        public override double RelativeLocation
        {
            get { return relativeLocation; }
        }

        public DomLocation(IEnumerable<int> elementPath, int textOffset, double relativeLocation)
        {
            this.elementPath = elementPath.ToArray();
            this.TextOffset = textOffset;
            this.relativeLocation = relativeLocation;
        }

        public override int CompareTo(ChapterLocation otherLocation)
        {
            if (otherLocation is DomLocation)
            {
                DomLocation other = otherLocation as DomLocation;

                int thisIndex;
                int otherIndex;

                int thisLength = elementPath.Length;
                int otherLength = other.elementPath.Length;
                int maxCounter = Math.Min(thisLength, otherLength);

                // Compare the element paths
                for (int i = 0; i < maxCounter; i++)
                {
                    thisIndex = elementPath[i];
                    otherIndex = other.elementPath[i];

                    if (thisIndex != otherIndex)
                    {
                        return thisIndex < otherIndex ? -1 : 1;
                    }
                }

                // The element paths have been the same so far
                if (thisLength != otherLength)
                {
                    return thisLength < otherLength ? -1 : 1;
                }

                // Same length, only offsets left to compare
                if (TextOffset != other.TextOffset)
                {
                    return TextOffset < other.TextOffset ? -1 : 1;
                }

                // Perfectly equal
                return 0;
            }
            else
            {
                return base.CompareTo(otherLocation);
            }
        }
    }
}
