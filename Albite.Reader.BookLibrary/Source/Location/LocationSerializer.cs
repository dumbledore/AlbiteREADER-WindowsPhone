using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Core.Json;
using Albite.Reader.Core.Serialization;
using System;

namespace Albite.Reader.BookLibrary
{
    internal static class LocationSerializer
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

        public static ISerializer<object> CreateSerializer()
        {
            return new JsonSerializer<object>(expectedTypes);
        }
    }
}