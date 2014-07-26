using Albite.Reader.BookLibrary;
using Albite.Reader.Container.Epub;
using Albite.Reader.Core.IO;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.BookLibrary.Test
{
    public static class LibraryHelper
    {
        public static async Task<Book> AddEpubFromResource(string resourcePath, Library library)
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
                            return await library.Books.AddAsync(epub, CancellationToken.None, null);
                        }
                    }
                }
            }
        }
    }
}
