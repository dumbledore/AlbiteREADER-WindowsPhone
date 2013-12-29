using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Core.Json;
using SvetlinAnkov.Albite.Core.Serialization;
using System;

namespace SvetlinAnkov.Albite.BookLibrary
{
    internal class LocationSerializer: IAlbiteSerializer<object>
    {
        private static readonly Type[] expectedTypes = new Type[]
        {
            typeof(ChapterLocation),
            typeof(FirstPageLocation),
            typeof(LastPageLocation),
            typeof(PageLocation),
            typeof(ElementLocation),
            typeof(DomLocation),
            typeof(BookLocation),
            typeof(RelativeChapterLocation),
            typeof(HistoryStack.SerializedHistoryStack),
        };

        private readonly JsonSerializer<object> serializer;

        public LocationSerializer()
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