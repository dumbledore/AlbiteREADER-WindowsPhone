using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Albite.Reader.Core.Xml.Atom
{
    public class AtomReader
    {
        public IAtomFeed Feed { get; private set; }

        public AtomReader(Stream stream, Uri baseUri = null)
        {
            Feed = processDocument(getDocument(stream), baseUri);
        }

        private XDocument getDocument(Stream stream)
        {
            // Setup the reader so that XDocument won't waste time on
            // comments and whitespace.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = new FixedXmlReader(stream, settings))
            {
                return XDocument.Load(reader);
            }
        }

        // Atom Namespace
        private static readonly XNamespace Namespace = XNamespace.Get("http://www.w3.org/2005/Atom");

        // Elements
        private static readonly XName FeedElementName = XName.Get("feed", Namespace.NamespaceName);
        private static readonly XName EntryElementName = XName.Get("entry", Namespace.NamespaceName);
        private static readonly XName LinkElementName = XName.Get("link", Namespace.NamespaceName);
        private static readonly XName TitleElementName = XName.Get("title", Namespace.NamespaceName);
        private static readonly XName AuthorElementName = XName.Get("author", Namespace.NamespaceName);
        private static readonly XName NameElementName = XName.Get("name", Namespace.NamespaceName);

        // Attributes
        private static readonly XName RelAttributeName = XName.Get("rel");
        private static readonly XName TypeAttributeName = XName.Get("type");
        private static readonly XName TitleAttributeName = XName.Get("title");
        private static readonly XName HrefAttributeName = XName.Get("href");

        // xml:base
        private static readonly XNamespace XmlNamespace = XNamespace.Get("http://www.w3.org/XML/1998/namespace");
        private static readonly XName XmlBaseAttributeName = XName.Get("base", XmlNamespace.NamespaceName);

        private IAtomFeed processDocument(XDocument doc, Uri baseUri)
        {
            if (doc.BaseUri != null && doc.BaseUri != string.Empty)
            {
                baseUri = new Uri(doc.BaseUri);
            }

            XElement rootElement = doc.Root;

            if (rootElement.Name != FeedElementName
                && rootElement.Name != EntryElementName)
            {
                throw new AtomException("Invalid root element name " + rootElement.Name);
            }

            return processFeed(rootElement, baseUri);
        }

        private AtomFeed processFeed(XElement element, Uri baseUri)
        {
            // The root is always a kind of entry (feed is also a an entry)
            AtomEntry feedEntry = processEntry(element, baseUri);

            AtomEntry[] entries = new AtomEntry[0];

            if (element.Name == FeedElementName)
            {
                // It's a feed, so look for entries
                List<AtomEntry> entriesList = new List<AtomEntry>();

                // Don't forget to get the baseUri as processEntry won't
                // have updated it, and we might need it for the entries
                tryGetBase(element, ref baseUri);

                foreach (XElement subElement in element.Elements(EntryElementName))
                {
                    entriesList.Add(processEntry(subElement, baseUri));
                }

                entries = entriesList.ToArray();
            }

            return new AtomFeed(feedEntry, entries);
        }

        private AtomEntry processEntry(XElement element, Uri baseUri)
        {
            string title = null;
            string author = null;
            List<AtomLink> links = new List<AtomLink>();

            // Is there an xml:base attribute?
            tryGetBase(element, ref baseUri);

            foreach(XElement subElement in element.Elements())
            {
                if (subElement.Name == LinkElementName)
                {
                    links.Add(processLink(subElement, baseUri));
                }
                else if (subElement.Name == TitleElementName)
                {
                    title = subElement.Value;
                }
                else if (subElement.Name == AuthorElementName)
                {
                    XElement nameElement = subElement.Element(NameElementName);
                    if (nameElement != null)
                    {
                        author = nameElement.Value;
                    }
                }
            }

            return new AtomEntry(title, author, links.ToArray());
        }

        AtomLink processLink(XElement element, Uri baseUri)
        {
            string rel = null;
            string mimetype = null;
            string title = null;
            Uri uri = null;

            // Is there an xml:base attribute?
            tryGetBase(element, ref baseUri);

            // Try filling in data
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.Name == RelAttributeName)
                {
                    rel = attribute.Value;
                }
                else if (attribute.Name == TypeAttributeName)
                {
                    mimetype = attribute.Value;
                }
                else if (attribute.Name == TitleAttributeName)
                {
                    title = attribute.Value;
                }
                else if (attribute.Name == HrefAttributeName)
                {
                    // Even if the link Uri is an absolute one,
                    // Uri(Uri, string) would not throw an exception
                    // and would create a valid absolute uri, i.e.
                    // it will have the same effect as
                    // Uri(string, UriKind.Absolute)

                    uri = baseUri != null
                        ? new Uri(baseUri, attribute.Value)
                        : new Uri(attribute.Value);
                }
            }

            return new AtomLink(uri, title, rel, mimetype);
        }

        private bool tryGetBase(XElement element, ref Uri baseUri)
        {
            XAttribute attribute = element.Attribute(XmlBaseAttributeName);
            if (attribute != null)
            {
                baseUri = new Uri(attribute.Value, UriKind.Absolute);
                return true;
            }

            return false;
        }
    }
}
