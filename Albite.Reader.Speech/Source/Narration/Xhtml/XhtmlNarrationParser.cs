using Albite.Reader.Speech.Narration.Nodes;
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
        private static readonly string Tag = "XhtmlNarrationParser";

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

        public RootNode Parse()
        {
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            // Used for custom entities
            xmlSettings.DtdProcessing = DtdProcessing.Parse;
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

            public RootNode Parse()
            {
                // Get to a known state
                reset();

                // Get the body element
                XElement body = doc.Root.Elements(BodyElementName).First();

                string language = settings.BaseLanguage;

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

                // Root. Base language
                RootNode rootNode = new RootNode(language);

                // Base speed
                ProsodyNode speedNode = new ProsodyNode(settings.BaseSpeedRatio);
                rootNode.AddChild(speedNode);

                // Now start parsing down
                parse(body.FirstNode, speedNode);

                return rootNode;
            }

            private void reset()
            {
                path.Clear();
            }

            private void parse(XNode node, NarrationNode nNode)
            {
                int i = 0;
                while (node != null)
                {
                    path.Push(i);
                    processNode(node, nNode); // This will go down if needed
                    node = node.NextNode;
                    path.Pop();
                    i++;
                }
            }

            private void processNode(XNode node, NarrationNode nNode)
            {
                if (node is XText)
                {
                    // Text node
                    XText text = (XText)node;
                    XhtmlLocation location = new XhtmlLocation(path.Reverse().ToArray(), 0);
                    LocatedTextNode<XhtmlLocation> textNode = new LocatedTextNode<XhtmlLocation>(textNodeId++, text.Value, location);
                    nNode.AddChild(textNode);
                }
                else if (node is XElement)
                {
                    // Element
                    XElement element = (XElement)node;

                    // check for lang changes
                    string language = getLanguage(element);

                    if (language != null)
                    {
                        VoiceNode voiceNode = new VoiceNode(language);
                        nNode.AddChild(voiceNode);
                        nNode = voiceNode;
                    }

                    // Get the tag name
                    string tagName = element.Name.LocalName;

                    // process the tag
                    NarrationNode newNode = processTag(tagName, nNode);

                    // Go down
                    parse(element.FirstNode, newNode);
                }
            }

            private NarrationNode processTag(string tagName, NarrationNode nNode)
            {
                NarrationNode newNode = nNode;

                switch (tagName)
                {
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                        {
                            newNode = new ParagraphNode();
                            nNode.AddChild(newNode);
                            nNode.AddChild(new BreakNode(settings.HeadingAfterPause));
                            break;
                        }

                    case "i":
                    case "em":
                    case "b":
                    case "strong":
                    case "u":
                        {
                            newNode = new ProsodyNode(settings.EmphasisSpeedRatio);
                            nNode.AddChild(newNode);
                            break;
                        }

                    case "p":
                        newNode = new ParagraphNode();
                        nNode.AddChild(newNode);
                        break;

                    case "q":
                    case "blockquote":
                    case "pre":
                    case "code":
                        {
                            newNode = new ProsodyNode(settings.QuoteSpeedRatio);
                            nNode.AddChild(newNode);
                            break;
                        }

                    // TODO: ol, ul and li
                }

                return newNode;
            }

            private string getLanguage(XElement element)
            {
                XAttribute attribute = element.Attribute(LangAttributeName);
                return attribute != null ? attribute.Value : null;
            }
        }
    }
}
