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

namespace SvetlinAnkov.AlbiteREADER.Model.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/20/spec/OPF_2.0.1_draft.htm#Section2.0
    /// </summary>
    public class OpenPackageFile
    {
        public static string XmlNamespaceOpf { get { return "{http://www.idpf.org/2007/opf}"; } }
        public static string XmlNamespaceDc { get { return "{http://purl.org/dc/elements/1.1/}"; } }

        public string Title { get; private set; }
        public string Author { get; private set; }
        public string PublicationDate { get; private set; }
        public string Language { get; private set; }
        public string Rights { get; private set; }
        public string Publisher { get; private set; }

        private Dictionary<string, string> items = new Dictionary<string, string>();
        private List<string> spine = new List<string>();
        public string NcxPath { get; private set; }

        public OpenPackageFile(string filename, XDocument doc)
        {
            string xmlns = XmlNamespaceOpf;

            // Get the root and make certain the root has the correct name
            XElement rootElement = doc.Element(xmlns + "package");

            // process the metadata
            XElement metadataElement = rootElement.Element(xmlns + "metadata");
            XElement dcMetadataElement = metadataElement.Element(xmlns + "dc-metadata");
            processMetadata(dcMetadataElement != null ? dcMetadataElement : metadataElement);

            // process the manifest
            XElement manifestElement = rootElement.Element(xmlns + "manifest");
            processManifest(filename, manifestElement);

            // process the spine
            XElement spineElement = rootElement.Element(xmlns + "spine");
            processSpine(spineElement);
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
                    if (Author == null)
                    {
                        // Only use the first creator (that is an author of course)
                        XAttribute attribute = element.Attribute(authorAttributeName);
                        if (attribute == null || attribute.Value == "aut")
                        {
                            Author = element.Value;
                        }
                    }
                }
                else if (name == dateName)
                {
                    if (PublicationDate == null)
                    {
                        XAttribute attribute = element.Attribute(dateAttributeName);
                        if (attribute == null || attribute.Value == "publication")
                        {
                            PublicationDate = element.Value;
                        }
                    }
                }
                else if (name == languageName)
                {
                    if (Language == null)
                    {
                        Language = element.Value;
                    }
                }
                else if (name == rightsName)
                {
                    if (Rights == null)
                    {
                        Rights = element.Value;
                    }
                }
                else if (name == publisherName)
                {
                    if (Publisher == null)
                    {
                        Publisher = element.Value;
                    }
                }
            }
        }

        private void processManifest(string filename, XElement manifestElement)
        {
            string itemName = XmlNamespaceOpf + "item";

            // Note that all hrefs are relative to the opf path
            Uri rootUri = new Uri("/");
            Uri baseUri = new Uri(rootUri, filename);

            foreach (XElement element in manifestElement.Elements(itemName))
            {
                XAttribute id = element.Attribute("id");
                XAttribute href = element.Attribute("href");

                Uri uri = new Uri(baseUri, href.Value);
                items[id.Value] = rootUri.MakeRelativeUri(uri).ToString();
            }
        }

        private void processSpine(XElement spineElement)
        {
            XAttribute ncxAttribute = spineElement.Attribute("toc");
            NcxPath = items[ncxAttribute.Value];

            string itemrefName = XmlNamespaceOpf + "itemref";
            foreach (XElement element in spineElement.Elements(itemrefName))
            {
                XAttribute idref = element.Attribute("idref");
                spine.Add(idref.Value);
            }
        }

        public IEnumerable<string> ItemIds
        {
            get
            {
                return items.Keys;
            }
        }

        public string Item(string id)
        {
            if (items.ContainsKey(id))
            {
                return items[id];
            }

            return null;
        }

        public Boolean ContainsItem(string id)
        {
            return items.ContainsKey(id);
        }

        public IEnumerable<string> Spine
        {
            get { return spine; }
        }
    }
}
