using Albite.Reader.BookLibrary;
using Albite.Reader.Container.Epub;
using Albite.Reader.Core.IO;
using System.IO;

namespace Albite.Reader.BookLibrary.Test
{
    public static class LibraryHelper
    {
        public static Book AddEpubFromResource(string resourcePath, Library library)
        {
            using (ResourceStorage res = new ResourceStorage(resourcePath))
            {
                using (Stream inputStream = res.GetStream(FileAccess.Read))
                {
                    using (ZipContainer zip = new ZipContainer(inputStream))
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
