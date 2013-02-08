using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics;
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Model.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/20/spec/OPF_2.0.1_draft.htm#Section2.0
    /// </summary>
    public class OpenPackageFile : EpubXmlFile
    {
        public static string XmlNamespaceOpf { get { return "{http://www.idpf.org/2007/opf}"; } }
        public static string XmlNamespaceDc { get { return "{http://purl.org/dc/elements/1.1/}"; } }

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

        public IEnumerable<string> ItemIds
        {
            get { return items.Keys; }
        }

        public string Item(string id)
        {
            if (items.ContainsKey(id))
            {
                return items[id];
            }

            return null;
        }

        public bool ContainsItem(string id)
        {
            return items.ContainsKey(id);
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

            // process the spine
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

                string path = GetPathFor(href.Value);
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

                spine.Add(idref.Value);
            }

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
            string xmlnsDc = XmlNamespaceDc;
            string titleName = xmlnsDc + "title";
            string authorName = xmlnsDc + "creator";
            string dateName = xmlnsDc + "date";
            string languageName = xmlnsDc + "language";
            string rightsName = xmlnsDc + "rights";
            string publisherName = xmlnsDc + "publisher";

            string xmlnsOpf = XmlNamespaceOpf;
            string authorAttributeName = xmlnsOpf + "role";
            string dateAttributeName = xmlnsOpf + "event";

            foreach (XElement element in metadataElement.Elements())
            {
                string name = element.Name.ToString();

                if (name == titleName)
                {
                    // Only use the first title element
                    if (Title == null)
                    {
                        Title = element.Value;
                    }
                }
                else if (name == authorName)
                {
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
                }
                else if (name == dateName)
                {
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
                }
                else if (name == languageName)
                {
                    // Only use the first language
                    if (Language == null)
                    {
                        Language = element.Value;
                    }
                }
                else if (name == rightsName)
                {
                    // Only use the first rights element
                    if (Rights == null)
                    {
                        Rights = element.Value;
                    }
                }
                else if (name == publisherName)
                {
                    // Only use the first publisher
                    if (Publisher == null)
                    {
                        Publisher = element.Value;
                    }
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
