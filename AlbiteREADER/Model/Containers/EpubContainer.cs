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
using SvetlinAnkov.AlbiteREADER.Utils;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace SvetlinAnkov.AlbiteREADER.Model.Containers
{
    public class EpubContainer : BookContainer
    {
        /// <summary>
        /// For more info check http://idpf.org/epub/30/spec/epub30-ocf.html#sec-container-metainf
        /// </summary>
        private static readonly string containerPath = "META-INF/container.xml";
        private static readonly string rootFileName = "{urn:oasis:names:tc:opendocument:xmlns:container}rootfile";
        private static readonly string rootFileAttribute = "full-path";

        private static readonly string opfNamespace = "http://www.idpf.org/2007/opf";
        private List<string> items;
        public IList<string> Items
        {
            get
            {
                return new List<string>(items);
            }
        }

        private Dictionary<string, string> metaData;
        public IDictionary<string, string> MetaData
        {
            get
            {
                return new Dictionary<string, string>(metaData);
            }
        }

        public TableOfContents TableOfContents { get; private set; }
        public Spine Spine { get; private set; }

        public EpubContainer(IAlbiteContainer archive) : base(archive)
        {
            // Read the container and extract the location to the OPF
            string opfPath = readContainer();

            // Read the actual data of importance
            string ncxPath = readRootFile();

            // Reading of the NCX file is optional, so it shouldn't fail
            // in any case
            try
            {
                readNcx(ncxPath);
            }
            catch { }
        }

        public override void Install(AlbiteStorage outputStorage)
        {
            // Simply copy the entities to the storage
            IList<string> names = items;
            //foreach (string name in entityNames)
        }

        public override Stream Stream(string entityName)
        {
            // First check that this stream is there and/or is allowed to
            // be used at all.
            if (!items.Contains(entityName))
            {
                throw new BookContainerException("Entity not found in book");
            }

            return base.Stream(entityName);
        }

        /// <summary>
        /// Parses the container and returns the path to the main rootfile.
        /// </summary>
        private string readContainer()
        {
            try
            {
                // First get the stream to content.xml
                using (Stream stream = container.Stream(containerPath))
                {
                    // Now use LinQ to XML to read the name
                    XDocument doc = XDocument.Load(stream);
                    IEnumerable<XElement> rootElements = doc.Descendants(rootFileName);
                    XElement rootFileElement = rootElements.First();
                    return (string) rootFileElement.Attribute(rootFileAttribute);
                }
            }
            catch (Exception e)
            {
                throw new BookContainerException("Processing the container.xml failed", e);
            }
        }

        /// <summary>
        /// Reads the root file (usually content.opf), pointed to by META-INF/container.xml
        /// 
        /// This will fill in the metadata, the items and the spine.
        /// </summary>
        /// <returns>The path to the table of contents if such was provided</returns>
        private string readRootFile()
        {
            return null;
        }

        /// <summary>
        /// If filename is not null, it will try to parse the ncx file pointed by it.
        /// </summary>
        /// <param name="filename"></param>
        private void readNcx(string filename)
        {
            if (filename == null)
            {
                return;
            }


        }
    }
}
