using Albite.Reader.Speech.Narration.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    internal class XhtmlNarrationParser : IDisposable
    {
        private static readonly XNamespace XhtmlNamespace = XNamespace.Get("http://www.w3.org/1999/xhtml");
        private static readonly XName BodyElementName = XName.Get("body", XhtmlNamespace.NamespaceName);

        private static readonly XNamespace XmlNamespace = XNamespace.Get("http://www.w3.org/XML/1998/namespace");
        private static readonly XName LangAttributeName = XName.Get("lang", XmlNamespace.NamespaceName);

        private Stream stream;
        private NarrationSettings settings;

        public XhtmlNarrationParser(Stream stream, NarrationSettings settings)
        {
            this.stream = stream;
            this.settings = settings;
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public RootElement Parse()
        {
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            // Used for custom entities
            xmlSettings.DtdProcessing = DtdProcessing.Parse;

            // Disallow XmlReader to open any external resources
            xmlSettings.XmlResolver = null;

            // TODO: Check if comments are ignored in IE10
            xmlSettings.IgnoreComments = false;

            XmlReader reader = XmlReader.Create(stream, xmlSettings);
            XDocument doc = XDocument.Load(reader);
            Parser parser = new Parser(doc, settings);
            return parser.Parse();
        }

        private class Parser
        {
            private XDocument doc;
            private Stack<int> path = new Stack<int>();
            private int textNodeId = 0;

            private NarrationSettings settings;

            public Parser(XDocument doc, NarrationSettings settings)
            {
                this.doc = doc;
                this.settings = settings;
            }

            public RootElement Parse()
            {
                // Get to a known state
                reset();

                // Get the body element
                XElement body = doc.Root.Element(BodyElementName);

                string language = settings.BaseVoice.Language;

                // Go up from body to root and try retrieving the language
                for (XElement element = body; element != null; element = element.Parent)
                {
                    string lang = getLanguage(element);
                    if (lang != null)
                    {
                        language = lang;
                        break;
                    }
                }

                // Root
                RootElement rootElement = new RootElement(language);

                // Now start parsing down
                parse(body.FirstNode, rootElement);

                return rootElement;
            }

            private void reset()
            {
                path.Clear();
            }

            private void parse(XNode node, NarrationElement nElement)
            {
                int i = 0;
                while (node != null)
                {
                    path.Push(i);
                    processNode(node, nElement); // This will go down if needed
                    node = node.NextNode;
                    path.Pop();
                    i++;
                }
            }

            private void processNode(XNode node, NarrationElement nElement)
            {
                if (node is XText)
                {
                    // Text node
                    XText text = (XText)node;
                    XhtmlLocation location = new XhtmlLocation(path.Reverse().ToArray());
                    TextElement<XhtmlLocation> textElement = new TextElement<XhtmlLocation>(textNodeId++, text.Value, location);
                    nElement.AddChild(textElement);
                }
                else if (node is XElement)
                {
                    // Element
                    XElement element = (XElement)node;

                    // check for lang changes
                    string language = getLanguage(element);

                    if (language != null)
                    {
                        LanguageElement languageElement = new LanguageElement(language);
                        nElement.AddChild(languageElement);
                        nElement = languageElement;
                    }

                    // Get the tag name
                    string tagName = element.Name.LocalName;

                    // process the tag
                    NarrationElement newElement = processTag(tagName, nElement);

                    // Go down
                    parse(element.FirstNode, newElement);
                }
            }

            private NarrationElement processTag(string tagName, NarrationElement element)
            {
                NarrationElement newElement = element;

                switch (tagName)
                {
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                        {
                            newElement = new HeadingElement();
                            element.AddChild(newElement);
                            break;
                        }

                    case "i":
                    case "em":
                    case "b":
                    case "strong":
                    case "u":
                        {
                            newElement = new EmphasisElement();
                            element.AddChild(newElement);
                            break;
                        }

                    case "p":
                    case "td":
                        newElement = new ParagraphElement();
                        element.AddChild(newElement);
                        break;

                    case "q":
                    case "blockquote":
                    case "pre":
                    case "code":
                        {
                            newElement = new QuoteElement();
                            element.AddChild(newElement);
                            break;
                        }

                    // TODO: ol, ul and li
                }

                return newElement;
            }

            private string getLanguage(XElement element)
            {
                XAttribute attribute = element.Attribute(LangAttributeName);
                return attribute != null ? attribute.Value : null;
            }
        }
    }
}
