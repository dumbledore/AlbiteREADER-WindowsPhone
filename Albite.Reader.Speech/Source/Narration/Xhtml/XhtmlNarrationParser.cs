using Albite.Reader.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private string defaultLanguage;

        public XhtmlNarrationParser(Stream stream, string defaultLanguage)
        {
            this.stream = stream;
            this.defaultLanguage = defaultLanguage;
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public INarrationCommand Parse()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            // Used for custom entities
            settings.DtdProcessing = DtdProcessing.Parse;
            // TODO: Check if comments are ignored in IE10
            settings.IgnoreComments = false;

            XmlReader reader = XmlReader.Create(stream, settings);
            XDocument doc = XDocument.Load(reader);
            Parser parser = new Parser(doc, defaultLanguage);
            return parser.Parse();
        }

        private class Parser
        {
            private XDocument doc;
            private Stack<int> path = new Stack<int>();

            private Stack<string> languages = new Stack<string>();

            private string defaultLanguage;

            private int emphasis = 0;
            private int paragraph = 0;
            private int header = 0;
            private int quote = 0;

            public Parser(XDocument doc, string defaultLanguage)
            {
                this.doc = doc;
                this.defaultLanguage = defaultLanguage;
            }

            public INarrationCommand Parse()
            {
                // Get to a known state
                reset();

                // Get the body element
                XElement body = doc.Root.Elements(BodyElementName).First();

                string language = defaultLanguage;

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

                // Base language. Should never get popped out.
                languages.Push(language);

                // Now start parsing down
                parse(body.FirstNode);

                // TODO: return the first of the created commands
                return null;
            }

            private void reset()
            {
                path.Clear();
                languages.Clear();
                emphasis = 0;
                paragraph = 0;
                header = 0;
                quote = 0;
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
                    XText text = (XText)node;
                    dumpTextNode(text);
                    // TODO
                }
                else if (node is XElement)
                {
                    // Element
                    XElement element = (XElement)node;

                    // check for lang changes
                    string language = getLanguage(element);

                    if (language != null)
                    {
                        languages.Push(language);
                    }

                    // Get the tag name
                    string tagName = element.Name.LocalName;

                    // get the command type once
                    CommandType commandType = getCommand(tagName);

                    // process the tag before going down
                    processTag(commandType, true);

                    // Go down
                    parse(element.FirstNode);

                    // now reverse the effect
                    processTag(commandType, false);

                    // Language no longer in effect
                    if (language != null)
                    {
                        languages.Pop();
                    }
                }
            }

            private void processTag(CommandType commandType, bool starting)
            {
                switch (commandType)
                {
                    case CommandType.Heading:
                        // read slower. add a pause after
                        header += starting ? +1 : -1;
                        break;

                    case CommandType.Paragraph:
                        // Add a pause after
                        paragraph += starting ? +1 : -1; 
                        break;

                    case CommandType.Empasis:
                        emphasis += starting ? +1 : -1;
                        break;

                    case CommandType.Quote:
                        // read like a quote
                        quote += starting ? +1 : -1;
                        break;
                }
            }

            private CommandType getCommand(string tagName)
            {
                CommandType commandType = CommandType.Unknown;

                switch (tagName)
                {
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                        commandType = CommandType.Heading;
                        break;

                    case "i":
                    case "em":
                    case "b":
                    case "strong":
                    case "u":
                        commandType = CommandType.Empasis;
                        break;

                    case "p":
                        commandType = CommandType.Paragraph;
                        break;

                    case "q":
                    case "blockquote":
                    case "pre":
                    case "code":
                        commandType = CommandType.Quote;
                        break;

                    // TODO: ol, ul and li
                }

                return commandType;
            }

            private string getLanguage(XElement element)
            {
                XAttribute attribute = element.Attribute(LangAttributeName);
                return attribute != null ? attribute.Value : null;
            }

            private void dumpTextNode(XText text)
            {
                StringBuilder b = new StringBuilder();
                IEnumerable<int> path_ = path.Reverse<int>();
                string path__ = string.Join<int>(", ", path_);

                b.Append("Text at [");
                b.Append(path__);
                b.Append("] with styles (");
                b.Append("e: ");
                b.Append(emphasis);
                b.Append(", h: ");
                b.Append(header);
                b.Append(", p: ");
                b.Append(paragraph);
                b.Append(", q: ");
                b.Append(quote);
                b.Append(") in `");
                b.Append(languages.Peek());
                b.Append("` {{");
                b.Append(text.Value);
                b.Append("}}");

                Log.D(Tag, b.ToString());
            }

            private enum CommandType
            {
                Unknown,
                Heading,
                Paragraph,
                Empasis,
                Quote,
            }
        }
    }
}
