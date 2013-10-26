using SvetlinAnkov.Albite.BookLibrary;

namespace SvetlinAnkov.Albite.READER
{
    public class AlbiteContext
    {
        public Library Library { get; private set; }

        public AlbiteContext(string libraryPath)
        {
            Library = new BookLibrary.Library(libraryPath);
        }
    }
}
