using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Speech.Narration.Commands;
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

        public NarrationCommand Parse()
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

            private Stack<string> languages = new Stack<string>();

            private NarrationSettings settings;

            private int emphasis = 0;
            private int paragraph = 0;
            private int heading = 0;
            private int quote = 0;

            private NarrationCommand currentCommand = null;

            public Parser(XDocument doc, NarrationSettings settings)
            {
                this.doc = doc;
                this.settings = settings;
            }

            public NarrationCommand Parse()
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

                // Base language. Should never get popped out.
                languages.Push(language);

                // Now start parsing down
                parse(body.FirstNode);

                // TODO: return the first of the created commands
                NarrationCommand rootCommand = currentCommand;

                if (rootCommand != null)
                {
                    while (rootCommand.Previous != null)
                    {
                        rootCommand = rootCommand.Previous;
                    }
                }

                return rootCommand;
            }

            private void addCommand(NarrationCommand command)
            {
                if (currentCommand != null)
                {
                    currentCommand.AddNext(command);
                }

                currentCommand = command;
            }

            private void reset()
            {
                path.Clear();
                languages.Clear();
                emphasis = 0;
                paragraph = 0;
                heading = 0;
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
                    processText(text);
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
                        addCommand(new LanguageCommand(language));
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

                        // This should never throw, as there is
                        // a default language
                        string previousLanguage = languages.Peek();
                        addCommand(new LanguageCommand(previousLanguage));
                    }
                }
            }

            // TODO: A better and more correct trimmer that will include
            // the separators *with* the expressions and also
            // will be able to filter out "empty" expressions

            private static readonly char[] Splitters
                = new char[] { '.', '?', '!', '\'', '"', ',', ':', '(', ')', '“', '”', };

            private void processText(XText textNode)
            {
                string text = textNode.Value;
                int pauseDuration = heading > 0 ? settings.HeadingSentencePause : settings.SentencePause;
                int offset = 0;


                // Split to "expressions"
                foreach (string ex in splitAndKeep(text, Splitters))
                {
                    string exTrimmed = ex.Trim();

                    if (exTrimmed.Length > 0)
                    {
                        XhtmlLocation location = new XhtmlLocation(path.Reverse().ToArray(), offset);
                        XhtmlNarrationExpression expression = new XhtmlNarrationExpression(exTrimmed, location);
                        addCommand(expression);
                        addCommand(new PauseCommand(pauseDuration));
                    }

                    offset += ex.Length;
                }
            }

            private static IEnumerable<string> splitAndKeep(string s, char[] delims)
            {
                int start = 0;
                int index = 0;
                int tmp = 0;

                while ((index = s.IndexOfAny(delims, start)) != -1)
                {
                    index++;
                    tmp = index;
                    index = start;
                    start = tmp;

                    yield return s.Substring(index, start - index - 1);
                    yield return s.Substring(start - 1, 1);
                }

                if (start < s.Length)
                {
                    yield return s.Substring(start);
                }
            }

            private void processTag(CommandType commandType, bool starting)
            {
                switch (commandType)
                {
                    case CommandType.Heading:
                        // read slower. add a pause after
                        processHeading(starting);
                        break;

                    case CommandType.Paragraph:
                        // Add a pause after
                        processParagraph(starting); 
                        break;

                    case CommandType.Empasis:
                        processEmphasis(starting);
                        break;

                    case CommandType.Quote:
                        // read like a quote
                        processQuote(starting);
                        break;
                }
            }

            private void processHeading(bool starting)
            {
                if (starting)
                {
                    heading++;
                }
                else
                {
                    if (--heading == 0)
                    {
                        // Just finished
                        addCommand(new PauseCommand(settings.HeadingAfterPause));
                    }
                }
            }

            private void processParagraph(bool starting)
            {
                if (starting)
                {
                    paragraph++;
                }
                else
                {
                    if (--paragraph == 0)
                    {
                        // Just finished
                        addCommand(new PauseCommand(settings.ParagraphAfterPause));
                    }
                }
            }

            private void processEmphasis(bool starting)
            {
                if (starting)
                {
                    if (emphasis++ == 0)
                    {
                        // Just started
                        adjustSpeed();
                    }
                }
                else
                {
                    if (--emphasis == 0)
                    {
                        // Just finished
                        adjustSpeed();
                    }
                }
            }

            private void processQuote(bool starting)
            {
                if (starting)
                {
                    if (quote++ == 0)
                    {
                        // Just started
                        adjustSpeed();
                    }
                }
                else
                {
                    if (--quote == 0)
                    {
                        // Just finished
                        adjustSpeed();
                    }
                }
            }

            private void adjustSpeed()
            {
                float speed = settings.BaseSpeedRatio;

                if (emphasis > 0)
                {
                    speed = settings.EmphasisSpeedRatio;
                }
                else if (quote > 0)
                {
                    speed = settings.QuoteSpeedRatio;
                }

                addCommand(new SpeedCommand(speed));
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
