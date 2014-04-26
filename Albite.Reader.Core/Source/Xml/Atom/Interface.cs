using System;
using System.Collections.Generic;

namespace Albite.Reader.Core.Xml.Atom
{
    public interface IAtomEntity
    {
        string Title { get; }
    }

    public interface IAtomLink : IAtomEntity
    {
        Uri Uri { get; }
        string Rel { get; }
        string Mimetype { get; }
    }

    public interface IAtomEntry : IAtomEntity
    {
        string Author { get; }
        IEnumerable<IAtomLink> Links { get; }
    }

    public interface IAtomFeed : IAtomEntry
    {
        IEnumerable<IAtomEntry> Entries { get; }
    }
}
