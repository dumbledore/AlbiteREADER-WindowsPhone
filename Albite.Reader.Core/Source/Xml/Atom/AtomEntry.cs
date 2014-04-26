using System;
using System.Collections.Generic;

namespace Albite.Reader.Core.Xml.Atom
{
    internal class AtomEntry : AtomEntity, IAtomEntry
    {
        public string Author { get; private set; }

        internal AtomLink[] links_ { get; private set; }

        public IEnumerable<IAtomLink> Links
        {
            get { return Array.AsReadOnly<IAtomLink>(links_); }
        }

        public AtomEntry(string title, string author, AtomLink[] links)
            : base(title)
        {
            Author = author;
            links_ = links;
        }
    }
}
