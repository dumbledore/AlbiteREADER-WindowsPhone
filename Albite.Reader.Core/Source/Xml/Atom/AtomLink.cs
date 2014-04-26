using System;

namespace Albite.Reader.Core.Xml.Atom
{
    internal class AtomLink : AtomEntity, IAtomLink
    {
        public Uri Uri { get; private set; }
        public string Rel { get; private set; }
        public string Mimetype { get; private set; }

        public AtomLink(Uri uri, string title, string rel, string mimetype)
            : base(title)
        {
            Uri = uri;
            Rel = rel;
            Mimetype = mimetype;
        }
    }
}
