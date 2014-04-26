using System;
using System.Collections.Generic;

namespace Albite.Reader.Core.Xml.Atom
{
    internal class AtomFeed : AtomEntry, IAtomFeed
    {
        private AtomEntry[] entries_;

        public IEnumerable<IAtomEntry> Entries
        {
            get { return Array.AsReadOnly<AtomEntry>(entries_); }
        }

        public AtomFeed(AtomEntry entry, AtomEntry[] entries)
            : base(entry.Title, entry.Author, entry.links_)
        {
            entries_ = entries;
        }

        public AtomFeed(string title, string author, AtomEntry[] entries, AtomLink[] links)
            : base(title, author, links)
        {
            entries_ = entries;
        }
    }
}