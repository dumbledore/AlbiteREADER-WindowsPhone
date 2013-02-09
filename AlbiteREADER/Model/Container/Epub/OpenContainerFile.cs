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
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.Model.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/30/spec/epub30-ocf.html#sec-container-metainf
    /// </summary>
    public class OpenContainerFile : EpubXmlFile
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
