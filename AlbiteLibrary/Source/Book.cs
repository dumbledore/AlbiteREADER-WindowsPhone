using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Library.DataContext;

namespace SvetlinAnkov.Albite.Library
{
    public class Book : LibraryEntity
    {
        public BookManager Manager { get; private set; }

        public string Title { get; private set; }

        internal Book(BookManager manager, BookEntity entity)
            : base(manager.Library, entity)
        {
            Manager = manager;

            // Other entity fields
            Title = entity.Title;
        }

        public sealed class Descriptor
        {
            public string Path { get; private set; }
            public BookContainerType Type { get; private set; }

            public Descriptor(string path, BookContainerType type)
            {
                Path = path;
                Type = type;
            }
        }
    }
}
