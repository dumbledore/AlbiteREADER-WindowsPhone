using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Json;
using SvetlinAnkov.Albite.Core.Serialization;
using System;

namespace SvetlinAnkov.Albite.BookLibrary
{
    internal class LibrarySerializer : IAlbiteSerializer<object>
    {
        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(DomLocation),
            typeof(BookLocation),
        };

        private readonly JsonSerializer<object> serializer;

        public LibrarySerializer()
        {
            serializer = new JsonSerializer<object>(expectedTypes);
        }

        public string Encode(object entity)
        {
            return serializer.Encode(entity);
        }

        public object Decode(string data)
        {
            return serializer.Decode(data);
        }
    }
}
