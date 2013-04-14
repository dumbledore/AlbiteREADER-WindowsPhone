using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;
using SvetlinAnkov.Albite.READER.Model.Container.Epub;

namespace SvetlinAnkov.Albite.Tests.Test
{
    public abstract class LibraryTest : TestCase
    {
        protected void AddBook(Library library, string epubPath)
        {
            Log("Opening ePub {0}", epubPath);

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(epubPath))
            {
                using (Stream inputStream = res.GetStream(FileAccess.Read))
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        // Create the ePub
                        using (EpubContainer epub = new EpubContainer(zip))
                        {
                            // Add to the library
                            library.Books.Add(epub);
                        }
                    }
                }
            }
        }
    }
}
