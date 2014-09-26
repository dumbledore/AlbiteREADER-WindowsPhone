using Albite.Reader.BookLibrary.Location;
using System;

namespace Albite.Reader.BookLibrary
{
    public interface IBookmark : IComparable<IBookmark>
    {
        BookLocation BookLocation { get; }
        string Text { get; }
    }
}
