using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "domLocation")]
    public class DomLocation : IComparable<DomLocation>
    {
        public static DomLocation Default = new DomLocation(new int[] { 0 }, 0);

        [DataMember(Name = "elementPath")]
        private int[] elementPath;
        public IList<int> ElementPath
        {
            get { return Array.AsReadOnly<int>(elementPath); }
        }

        [DataMember(Name = "textOffset")]
        public int TextOffset { get; private set; }

        public DomLocation(IList<int> elementPath, int textOffset)
        {
            this.elementPath = new int[elementPath.Count];
            elementPath.CopyTo(this.elementPath, 0);
        }

        public int CompareTo(DomLocation other)
        {
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

        public static DomLocation FromString(string encodedData)
        {
            if (encodedData == null)
            {
                return Default;
            }

            try
            {
                LibrarySerializer serializer = new LibrarySerializer();
                return (DomLocation)serializer.Decode(encodedData);
            }
            catch (Exception)
            {
                return Default;
            }
        }

        public override string ToString()
        {
            LibrarySerializer serializer = new LibrarySerializer();
            return serializer.Encode(this);
        }
    }
}
