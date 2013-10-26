using SvetlinAnkov.Albite.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SvetlinAnkov.Albite.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/30/spec/epub30-ocf.html#sec-container-metainf
    /// </summary>
    internal class OpenContainerFile : EpubXmlFile
    {
        public static string Path { get { return "META-INF/container.xml"; } }
        public static string XmlNamespace { get { return "{urn:oasis:names:tc:opendocument:xmlns:container}"; } }

        public string OpfPath { get; private set; }

        public OpenContainerFile(IAlbiteContainer container)
            : base(container, Path)
        {
            processDocument();
        }

        private void processDocument()
        {
            XDocument doc = GetDocument();

            IEnumerable<XElement> elements = doc.Descendants(XmlNamespace + "rootfile");
            Assert(elements.Count() > 0, "No rootfile element");

            XAttribute attribute = elements.First().Attribute("full-path");
            Assert(attribute, "No full-path attribute");

            OpfPath = attribute.Value;
        }
    }
}
