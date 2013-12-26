using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Core.Serialization;

namespace SvetlinAnkov.Albite.READER
{
    public class AlbiteContext
    {
        public Library Library { get; private set; }
        public RecordStore RecordStore { get; private set; }

        public AlbiteContext(string libraryPath, string recordStorePath)
        {
            Library = new BookLibrary.Library(libraryPath);
            RecordStore = new RecordStore(recordStorePath);
        }
    }
}
