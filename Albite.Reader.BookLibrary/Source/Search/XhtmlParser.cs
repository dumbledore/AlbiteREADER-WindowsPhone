using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Albite.Reader.BookLibrary.Search
{
    internal class XhtmlParser
    {
        private static readonly XNamespace XhtmlNamespace = XNamespace.Get("http://www.w3.org/1999/xhtml");
        private static readonly XName BodyElementName = XName.Get("body", XhtmlNamespace.NamespaceName);

        private string query;
        private Chapter chapter;
        private CancellationToken cancelToken;

        public XhtmlParser(string query, Chapter chapter, CancellationToken cancelToken)
        {
            this.query = query;
            this.chapter = chapter;
            this.cancelToken = cancelToken;
        }

        public IEnumerable<IBookmark> Parse()
        {
            Parser parser = getParser();

            // cache total length as double
            double total = parser.TotalLength;

            // current offset
            int offset = 0;

            List<IBookmark> results = new List<IBookmark>();

            foreach (Result result in parser.Results)
            {
                // Get chapter relative location ratio
                double relativeLocation = result.GlobalOffset / total;

                // Create DOM location
                DomLocation domLocation = new DomLocation(result.ElementPath, result.LocalOffset, relativeLocation);

                // And now BookLocation
                BookLocation bookLocation = chapter.CreateLocation(domLocation);

                // And finally, the bookmark
                results.Add(new BookSeekerBookmark(bookLocation, result.Text));

                // Update the current offset
                offset += result.Text.Length;
            }

            return results.AsReadOnly();
        }

        private Parser getParser()
        {
            // Get the name of the chapter file
            string path = Path.Combine(chapter.BookPresenter.ContentPath, chapter.Url);

            XmlReaderSettings xmlSettings = new XmlReaderSettings();

            // Used for custom entities
            xmlSettings.DtdProcessing = DtdProcessing.Parse;

            // Disallow XmlReader to open any external resources
            xmlSettings.XmlResolver = null;

            // TODO: Check if comments are ignored in IE10
            xmlSettings.IgnoreComments = false;

            using (IsolatedStorage iso = new IsolatedStorage(path))
            {
                using (Stream stream = iso.GetStream(FileAccess.Read))
                {
                    using (XmlReader reader = XmlReader.Create(stream, xmlSettings))
                    {
                        XDocument doc = XDocument.Load(reader);
                        return new Parser(query, doc, cancelToken);
                    }
                }
            }
        }

        private class Parser
        {
            private string query;
            private XDocument doc;
            private CancellationToken cancelToken;
            private Stack<int> path = new Stack<int>();
            private List<Result> results_ = new List<Result>();

            public List<Result> Results
            {
                get { return results_; }
            }

            public int TotalLength { get; private set; }

            public Parser(string query, XDocument doc, CancellationToken cancelToken)
            {
                this.query = query;
                this.doc = doc;
                this.cancelToken = cancelToken;
            }

            public void Parse()
            {
                // Get to a known state
                reset();

                // Get the body element
                XElement body = doc.Root.Element(BodyElementName);

                // Now start parsing down
                parse(body.FirstNode);
            }

            private void parse(XNode node)
            {
                int i = 0;
                while (node != null)
                {
                    path.Push(i);
                    processNode(node); // This will go down if needed
                    node = node.NextNode;
                    path.Pop();
                    i++;
                }
            }

            private void processNode(XNode node)
            {
                if (node is XText)
                {
                    // Text node
                    processText((XText)node);
                }
                else if (node is XElement)
                {
                    // Element
                    XElement element = (XElement)node;

                    // Go down
                    parse(element.FirstNode);
                }
            }

            private void reset()
            {
                path.Clear();
                results_.Clear();
                TotalLength = 0;
            }

            private void processText(XText node)
            {
                // cancel if needed
                cancelToken.ThrowIfCancellationRequested();

                // node's cached path
                int[] p = null;

                // cache node's text content
                string text = node.Value;

                // keep searching for this string
                for (int i = indexOf(text); i >= 0; i = indexOf(text, i))
                {
                    // cancel if needed
                    cancelToken.ThrowIfCancellationRequested();

                    if (p == null)
                    {
                        // cache the path
                        p = path.Reverse().ToArray();
                    }

                    // found one
                    results_.Add(new Result(p, TotalLength, i, getText(text, i)));
                }

                // Add to total length
                TotalLength += text.Length;
            }

            private int indexOf(string text, int start = 0)
            {
                return text.IndexOf(query, start);
            }

            public static readonly int TextRadius = 50;

            private string getText(string text, int offset)
            {
                int start = Math.Max(0, offset - TextRadius);
                int remaining = Math.Max(0, text.Length - offset);
                int length = Math.Min(remaining, TextRadius);
                return text.Substring(start, length);
            }
        }

        private class Result
        {
            public int[] ElementPath { get; private set; }

            public int GlobalOffset { get; private set; }

            public int LocalOffset { get; private set; }

            public string Text { get; private set; }

            public Result(int[] elementPath, int globalOffset, int localOffset, string text)
            {
                this.ElementPath = elementPath;
                this.GlobalOffset = globalOffset;
                this.LocalOffset = localOffset;
                this.Text = text;
            }
        }
    }
}
