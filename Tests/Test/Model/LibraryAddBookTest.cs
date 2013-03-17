using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.READER.Model;
using System.IO;
using SvetlinAnkov.Albite.READER.Model.Container.Epub;
using SvetlinAnkov.Albite.READER.Model.Container;

namespace SvetlinAnkov.Albite.Tests.Test.Model
{
    public class LibraryAddBookTest : TestCase
    {
        private string location;
        private string[] books;

        public LibraryAddBookTest(string location, string[] books)
        {
            this.location = location;
            this.books = books;
        }

        protected override void TestImplementation()
        {
            // Start using the library
            using (Library library = new Library(location))
            {
                foreach (string book in books)
                {
                    // Add a book
                    addBook(library, book);
                }
            }
        }

        private void addBook(Library library, string epubPath)
        {
            Log("Opening ePub {0}", epubPath);

            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(epubPath))
            {
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(epubPath))
                {
                    res.CopyTo(iso);
                }

                using (Stream inputStream = iso.GetStream(FileAccess.Read))
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        // Create the ePub
                        EpubContainer epub = new EpubContainer(zip);

                        // Add to the library
                        library.Books.Add(epub);
                    }
                }
            }
        }
    }
}
