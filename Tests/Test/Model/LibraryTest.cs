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
        public sealed class Descriptor
        {
            public string Path { get; private set; }
            public Book.ContainerType Type { get; private set; }

            public Descriptor(
                string path, Book.ContainerType type)
            {
                Path = path;
                Type = type;
            }
        }

        protected void AddBook(Library library, Descriptor descriptor)
        {
            Log("Opening book {0}", descriptor.Path);

            using (AlbiteResourceStorage res = new AlbiteResourceStorage(descriptor.Path))
            {
                using (Stream inputStream = res.GetStream(FileAccess.Read))
                {
                    using (AlbiteZipContainer zip = new AlbiteZipContainer(inputStream))
                    {
                        using (Book.Descriptor bookDescriptor
                            = new Book.Descriptor(zip, descriptor.Type))
                        {
                            // Add to the library
                            library.Books.Add(bookDescriptor);
                        }
                    }
                }
            }
        }
    }
}
