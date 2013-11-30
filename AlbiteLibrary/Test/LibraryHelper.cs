using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Container.Epub;
using SvetlinAnkov.Albite.Core.IO;
using System.IO;

namespace SvetlinAnkov.Albite.BookLibrary.Test
{
    public static class LibraryHelper
    {
        public static Book AddEpubFromResource(string resourcePath, Library library)
        {
            using (AlbiteResourceStorage res = new AlbiteResourceStorage(resourcePath))
            {
                using (Stream inputStream = res.GetStream(FileAccess.Read))
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        using (EpubContainer epub = new EpubContainer(zip))
                        {
                            // Add to the library
                            return library.Books.Add(epub);
                        }
                    }
                }
            }
        }
    }
}
