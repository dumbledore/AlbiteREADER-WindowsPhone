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
        internal Book(Library library, BookEntity entity)
            : base(library, entity)
        {
            // TODO: Reading persistance
        }

        public override void Remove()
        {
            Library.Books.Remove(this);
        }

        public BookPresenter GetPresenter()
        {
            return new BookPresenter(this);
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
