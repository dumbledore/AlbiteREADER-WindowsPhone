using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SvetlinAnkov.Albite.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/20/spec/OPF_2.0.1_draft.htm#Section2.0
    /// </summary>
    internal class OpenPackageFile : EpubXmlFile
    {
        public const string XmlNamespaceOpf = "{http://www.idpf.org/2007/opf}";
        public const string XmlNamespaceDc = "{http://purl.org/dc/elements/1.1/}";

        public string Title { get; private set; }
        public string Author { get; private set; }
        public string PublicationDate { get; private set; }
        public string Language { get; private set; }
        public string Rights { get; private set; }
        public string Publisher { get; private set; }

        public string NcxPath { get; private set; }

        /// <summary>
        /// Returns true if there was a problem with parsing the file.
        /// </summary>
        public bool HadErrors { get; private set; }

        public IEnumerable<string> Spine { get; private set; }

        private static readonly string tag = "OpenPackageFile";
        private Dictionary<string, string> items = new Dictionary<string, string>();

        public OpenPackageFile(IAlbiteContainer container, string filename)
            : base(container, filename)
        {
            HadErrors = false;
            processDocument();
        }

        public IEnumerable<string> Items
        {
            get { return items.Values; }
        }

        public IEnumerable<string> ItemIds
        {
            get { return items.Keys; }
        }

        /// <summary>
        /// Returns the item path for the specified id
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>item path</returns>
        public string Item(string id)
        {
            if (items.ContainsKey(id))
            {
                return items[id];
            }

            return null;
        }

        /// <summary>
        /// Checks if there is an item with
        /// the specified path.
        /// </summary>
        /// <param name="path">item path</param>
        /// <returns>true if there is an item with that path</returns>
        public bool ContainsItem(string path)
        {
            return items.ContainsValue(path);
        }

        // IMPLEMENTATION
        private void processDocument()
        {
            string xmlns = XmlNamespaceOpf;

            XDocument doc = GetDocument();

            // Get the root and check if the root has the correct name
            XElement rootElement = doc.Root;
            if (rootElement.Name != xmlns + "package")
            {
                reportError("Root element for opf has an incorrect name: " + rootElement.Name);
            }

            // process the manifest
            XElement manifestElement = rootElement.Element(xmlns + "manifest");
            processManifest(manifestElement);

            // process the spine last so that it will be able to use the manifest
            XElement spineElement = rootElement.Element(xmlns + "spine");
            processSpine(spineElement);

            try
            {
                // process the metadata. not critical
                XElement metadataElement = rootElement.Element(xmlns + "metadata");
                Assert(metadataElement, "No metadata element");

                XElement dcMetadataElement = metadataElement.Element(xmlns + "dc-metadata");
                processMetadata(dcMetadataElement != null ? dcMetadataElement : metadataElement);
            }
            catch (Exception e)
            {
                reportError("Failed processing the metadata", e);
            }
        }

        private void processManifest(XElement manifestElement)
        {
            Assert(manifestElement, "No manifest element");

            string itemName = XmlNamespaceOpf + "item";

            // Note that all hrefs are relative to the opf path
            foreach (XElement element in manifestElement.Elements(itemName))
            {
                XAttribute id = element.Attribute("id");
                XAttribute href = element.Attribute("href");

                if (id == null || href == null)
                {
                    reportError("id or href not found for item");
                    continue;
                }

                string path = UriResolver.ResolveToString(href.Value);
                if (!IsValidFileName(path))
                {
                    reportError("href not a valid filename: " + path);
                    continue;
                }

                items[id.Value] = path;
            }
        }

        private void processSpine(XElement spineElement)
        {
            Assert(spineElement, "No spine element");

            setNcxPath(spineElement);

            List<string> spine = new List<string>();

            string itemrefName = XmlNamespaceOpf + "itemref";
            foreach (XElement element in spineElement.Elements(itemrefName))
            {
                XAttribute idref = element.Attribute("idref");
                if (idref == null)
                {
                    reportError("itemref without a valid idref");
                    continue;
                }

                // According to the OPF specification:
                // 1. A single resource (href) must not be listed in the manifest more than once.
                // 2. In addition, a specific spine item (from the perspective of its id attribute
                //    value in manifest) must not appear more than once in spine.
                // Therefore:
                // 1. every href can be included only once in the manifest, thus it can have
                //    only one id associated with it. That is, there can be two different ids
                //    pointing to the same resource.
                // 2. Every spine item with the same id must be featured only once in the spine
                if (items.ContainsKey(idref.Value))
                {
                    spine.Add(items[idref.Value]);
                }
                else
                {
                    reportError("Couldn't add " + idref.Value + " to spine");
                }
            }

            Assert(spine.Count > 0, "Spine is empty");

            Spine = spine;
        }

        private void setNcxPath(XElement spineElement)
        {
            XAttribute ncxAttribute = spineElement.Attribute("toc");
            if (ncxAttribute == null)
            {
                reportError("Spine element doesn't specify a toc attribute");
                return;
            }

            if (!items.ContainsKey(ncxAttribute.Value))
            {
                reportError("Couldn't find the path for the ncx with id "
                    + ncxAttribute.Value);
                return;
            }

            NcxPath = items[ncxAttribute.Value];
        }

        private void processMetadata(XElement metadataElement)
        {
            const string xmlnsDc = XmlNamespaceDc;
            const string titleName = xmlnsDc + "title";
            const string authorName = xmlnsDc + "creator";
            const string dateName = xmlnsDc + "date";
            const string languageName = xmlnsDc + "language";
            const string rightsName = xmlnsDc + "rights";
            const string publisherName = xmlnsDc + "publisher";

            const string xmlnsOpf = XmlNamespaceOpf;
            const string authorAttributeName = xmlnsOpf + "role";
            const string dateAttributeName = xmlnsOpf + "event";

            foreach (XElement element in metadataElement.Elements())
            {
                string name = element.Name.ToString();

                switch (name)
                {
                    case titleName:
                        // Only use the first title element
                        if (Title == null)
                        {
                            Title = element.Value;
                        }
                        break;

                    case authorName:
                        // Only use the first creator (that is an author of course)
                        if (Author == null)
                        {
                            XAttribute attribute = element.Attribute(authorAttributeName);
                            // Either no attribute or its value must be "aut"
                            if (attribute == null || attribute.Value == "aut")
                            {
                                Author = element.Value;
                            }
                        }
                        break;

                    case dateName:
                        // Only use the first publication date
                        if (PublicationDate == null)
                        {
                            XAttribute attribute = element.Attribute(dateAttributeName);
                            // Either no attribute or its value must be "publication"
                            if (attribute == null || attribute.Value == "publication")
                            {
                                PublicationDate = element.Value;
                            }
                        }
                        break;

                    case languageName:
                        // Only use the first language
                        if (Language == null)
                        {
                            Language = element.Value;
                        }
                        break;

                    case rightsName:
                        // Only use the first rights element
                        if (Rights == null)
                        {
                            Rights = element.Value;
                        }
                        break;

                    case publisherName:
                        // Only use the first publisher
                        if (Publisher == null)
                        {
                            Publisher = element.Value;
                        }
                        break;
                }
            }
        }

        private void reportError(string msg)
        {
            Log.E(tag, msg);
            HadErrors = true;
        }

        private void reportError(string msg, Exception e)
        {
            Log.E(tag, msg, e);
            HadErrors = true;
        }
    }
}
