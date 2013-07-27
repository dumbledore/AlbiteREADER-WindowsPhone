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
using SvetlinAnkov.Albite.Core.Utils;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using SvetlinAnkov.Albite.Core.Utils.Xml;

namespace SvetlinAnkov.Albite.READER.Model.Container.Epub
{
    internal abstract class EpubXmlFile
    {
        protected IAlbiteContainer Container { get; private set; }
        protected string Filename { get; private set; }

        private Uri baseUri;

        private static Uri rootUri = new Uri("file:///");

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
        public Uri GetUriFor(string path)
        {
            Uri uri = new Uri(baseUri, path);
            return rootUri.MakeRelativeUri(uri);
        }

        /// <summary>
        /// Returns the unescaped path of the resource relative to the base path
        /// </summary>
        /// <param name="path">The path of the resource</param>
        public string GetPathFor(string path)
        {
            return Uri.UnescapeDataString(GetUriFor(path).ToString());
        }

        /// <summary>
        /// Checks if the path is a valid file name
        /// </summary>
        public bool IsValidFileName(string path)
        {
            if (path == null || path.Length < 1)
            {
                return false;
            }

            char[] chars = path.ToCharArray();

            // Check path ending
            switch (chars[chars.Length - 1])
            {
                case '.':
                case '/':
                case '\\':
                    return false;
            }

            foreach (char c in chars)
            {
                // bad chars
                switch (c)
                {
                    case '"':
                    case '*':
                    case ':':
                    case '<':
                    case '>':
                    case '?':
                    case '\u007F':
                        return false;
                }

                // bad ranges
                if (
                    ('\u0000' <= c && c <= '\u001F') || // C0 range
                    ('\u0080' <= c && c <= '\u009F') || // C1 range
                    ('\uE000' <= c && c <= '\uF8FF') || // Private Use Area
                    ('\uFDD0' <= c && c <= '\uFDEF') || // Non characters in Arabic Presentation Forms-A
                    ('\uFFF0' <= c && c <= '\uFFFF')    // Specials
                    )
                {
                    return false;
                }
            }
            return true;
        }
    }
}
