using SvetlinAnkov.Albite.Core.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    [DataContract(Name = "domLocation")]
    public class DomLocation
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

        public static DomLocation FromString(string encodedData)
        {
            if (encodedData == null)
            {
                return Default;
            }

            try
            {
                return (DomLocation)serializer.Decode(encodedData);
            }
            catch (Exception)
            {
                return Default;
            }
        }

        public override string ToString()
        {
            return serializer.Encode(this);
        }

        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(DomLocation),
        };

        private static readonly JsonSerializer<object> serializer
            = new JsonSerializer<object>(expectedTypes);

    }
}
