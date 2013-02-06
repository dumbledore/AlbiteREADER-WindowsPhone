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
using System.IO;
using System.Xml;
using SvetlinAnkov.AlbiteREADER.Utils.Xml;

namespace SvetlinAnkov.AlbiteREADER.Model.Container.Epub
{
    public abstract class EpubXmlFile
    {
        protected IAlbiteContainer Container { get; private set; }
        protected string Filename { get; private set; }

        private Uri rootUri = new Uri("/");
        private Uri baseUri;

        public EpubXmlFile(IAlbiteContainer container, string filename)
        {
            Container = container;
            Filename = filename;

            baseUri = new Uri(rootUri, filename);
        }

        protected XDocument GetDocument()
        {
            using (Stream stream = Container.Stream(Filename))
            {
                // Setup the reader so that XDocument won't waste time on
                // comments and whitespace.
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

                using (XmlReader reader = new AlbiteXmlReader(stream, settings))
                {
                    return XDocument.Load(reader);
                }
            }
        }

        protected void Assert(object o, string message)
        {
            Assert(o != null, message);
        }

        protected void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new BookContainerException(message);
            }
        }

        /// <summary>
        /// Returns the Uri of the resource relative to the base path
        /// </summary>
        /// <param name="path">The path of the resource</param>
        /// <returns></returns>
        public Uri GetUriFor(string path)
        {
            Uri uri = new Uri(baseUri, path);
            return rootUri.MakeRelativeUri(uri);
        }
    }
}
