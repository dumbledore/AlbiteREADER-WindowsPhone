using SvetlinAnkov.Albite.Core.Collections;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SvetlinAnkov.Albite.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/20/spec/OPF_2.0.1_draft.htm#Section2.4.1
    /// </summary>
    internal class NavigationControlFile : EpubXmlFile
    {
        public static string XmlNamespace { get { return "{http://www.daisy.org/z3986/2005/ncx/}"; } }

        public NavMap NavigationMap { get; private set; }
        public IEnumerable<NavList> NavigationLists { get; private set; }
        public IEnumerable<GuideReference> GuideReferences { get; private set; }

        /// <summary>
        /// Returns true if there was a problem with parsing the file.
        /// </summary>
        public bool HadErrors { get; private set; }

        private static readonly string tag = "NavigationControlFile";

        private NavObject.GetPathForDelegate getPathForDelegate;
        private NavObject.ReportErrorDelegate reportErrorDelegate;

        public NavigationControlFile(IAlbiteContainer container, string filename)
            : base(container, filename)
        {
            getPathForDelegate = new NavObject.GetPathForDelegate(getPathFor);
            reportErrorDelegate = new NavObject.ReportErrorDelegate(reportError);
            HadErrors = false;
            processDocument();
        }

        public class NavMap : NavObject, ITree<IContentItem>
        {
            public static readonly string ElementName = XmlNamespace + "navMap";

            // ITree
            public INode<IContentItem> Root { get; private set; }

            public IEnumerator<INode<IContentItem>> GetEnumerator()
            {
                return new DepthFirstTreeEnumerator<IContentItem>(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public NavMap(XElement element, ReportErrorDelegate reportError, GetPathForDelegate getPath)
            {
                XElement pointElement = element.Element(NavPoint.ElementName);
                if (pointElement != null)
                {
                    Root = new NavPoint(pointElement, reportError, getPath, 0);
                }
            }
        }

        public class NavPoint : NavContent, INode<IContentItem>, IContentItem
        {
            public static readonly string ElementName = XmlNamespace + "navPoint";

            // IContentItem
            public string Title { get { return Label; } }
            public string Location { get { return Src; } }

            // INode<IContentItem>
            public INode<IContentItem> FirstChild { get; private set; }
            public INode<IContentItem> NextSibling { get; private set; }
            public int Depth { get; private set; }
            public IContentItem Value { get { return this; } }

            public NavPoint(XElement element, ReportErrorDelegate reportError, GetPathForDelegate getPath, int depth)
                : base(element, reportError, getPath)
            {
                Depth = depth;

                XElement child = element.Element(ElementName);
                if (child != null)
                {
                    FirstChild = new NavPoint(child, reportError, getPath, depth + 1);
                }

                IEnumerable<XElement> nextElements = element.ElementsAfterSelf(ElementName);
                if (nextElements.Count() > 0)
                {
                    NextSibling = new NavPoint(nextElements.First(), reportError, getPath, depth);
                }
            }
        }

        public class NavList : NavLabel
        {
            public static readonly string ElementName = XmlNamespace + "navList";

            public NavTarget FirstTarget { get; private set; }

            public NavList(XElement element, ReportErrorDelegate reportError, GetPathForDelegate getPath)
                : base(element, reportError)
            {
                XElement targetElement = element.Element(NavTarget.ElementName);
                if (targetElement != null)
                {
                    FirstTarget = new NavTarget(targetElement, reportError, getPath);
                }
            }
        }

        public class NavTarget : NavContent
        {
            public static readonly string ElementName = XmlNamespace + "navTarget";

            public NavTarget NextSibling { get; protected set; }

            public NavTarget(XElement element, ReportErrorDelegate reportError, GetPathForDelegate getPath)
                : base(element, reportError, getPath)
            {
                IEnumerable<XElement> nextElements = element.ElementsAfterSelf(ElementName);
                if (nextElements.Count() > 0)
                {
                    NextSibling = new NavTarget(nextElements.First(), reportError, getPath);
                }
            }
        }

        public abstract class NavContent : NavLabel
        {
            private static readonly string elementName = XmlNamespace + "content";
            private static readonly string attributeName = "src";

            public string Src { get; private set; }

            public NavContent(XElement element, ReportErrorDelegate reportError, GetPathForDelegate getPath)
                : base(element, reportError)
            {
                XElement contentElement = element.Element(elementName);
                if (contentElement == null)
                {
                    if (reportError != null)
                    {
                        reportError("no content element");
                    }
                    return;
                }

                XAttribute srcAttribute = contentElement.Attribute(attributeName);
                if (srcAttribute == null)
                {
                    if (reportError != null)
                    {
                        reportError("no src attribute");
                    }
                    return;
                }

                Src = srcAttribute.Value;

                // If the GetPath delegate is available, pass the path through it.
                // It doesn't look perfectly readable this way, but it's
                // the way to get our abstraction.
                if (getPath != null)
                {
                    Src = getPath(Src);
                }
            }
        }

        public abstract class  NavLabel : NavObject
        {
            private static readonly string labelName = XmlNamespace + "navLabel";
            private static readonly string textName = XmlNamespace + "text";

            public string Label { get; private set; }

            public NavLabel(XElement element, ReportErrorDelegate reportError)
            {
                XElement labelElement = element.Element(labelName);
                if (labelElement == null)
                {
                    if (reportError != null)
                    {
                        reportError("no label element");
                    }
                    return;
                }

                XElement textElement = labelElement.Element(textName);
                if (textElement == null)
                {
                    if (reportError != null)
                    {
                        reportError("No text element");
                    }
                    return;
                }

                Label = textElement.Value;
            }
        }

        public abstract class NavObject
        {
            /// <summary>
            /// Report an error.
            /// </summary>
            /// <param name="msg">error message</param>
            public delegate void ReportErrorDelegate(string msg);

            /// <summary>
            /// Gets the Uri path for a path.
            /// </summary>
            /// <param name="path">path</param>
            /// <returns></returns>
            public delegate string GetPathForDelegate(string path);
        }

        public enum GuideType
        {
            Cover,          // the book cover(s), jacket information, etc.
            TitlePage,      // page with possibly title, author, publisher, and other metadata
            Toc,            // Table of Contents
            Index,          // back-of-book style index
            Glossary,
            Acknowledgements,
            Bibliography,
            Colophon,
            CopyrightPage,
            Dedication,
            Epigraph,
            Foreword,
            Loi,            // List of Illustrations
            Lot,            // List of Tables
            Notes,
            Preface,
            Text,           // First "real" page of content (e.g. "Chapter 1")
            Unknown,
        }

        public class GuideReference : NavObject
        {
            public static readonly string ElementName = XmlNamespace + "reference";

            public GuideType GuideType { get; private set; }
            public string Title { get; private set; }
            public string Href { get; private set; }

            public GuideReference(XElement element, ReportErrorDelegate reportError, GetPathForDelegate getPath)
            {
                GuideType = GuideType.Unknown;

                XAttribute typeAttribute = element.Attribute("type");
                if (typeAttribute == null)
                {
                    if (reportError != null)
                    {
                        reportError("no type attribute");
                    }
                }
                else
                {
                    string guideType = typeAttribute.Value;
                    try
                    {
                        // Need to handle those two cases rather ugly
                        if (guideType.Equals("title-page", StringComparison.OrdinalIgnoreCase))
                        {
                            GuideType = GuideType.TitlePage;
                        }
                        else if (guideType.Equals("copyright-page", StringComparison.OrdinalIgnoreCase))
                        {
                            GuideType = GuideType.CopyrightPage;
                        }
                        else
                        {
                            GuideType = (GuideType)Enum.Parse(typeof(GuideType), guideType, true);
                        }
                    }
                    catch (Exception)
                    {
                        if (reportError != null)
                        {
                            reportError("unknown guide type " + guideType);
                        }
                    }
                }

                XAttribute titleAttribute = element.Attribute("title");
                if (titleAttribute == null)
                {
                    if (reportError != null)
                    {
                        reportError("no title attribute for guide ref");
                    }
                }
                else
                {
                    Title = titleAttribute.Value;
                }

                XAttribute hrefAttribute = element.Attribute("href");
                if (hrefAttribute == null)
                {
                    if (reportError != null)
                    {
                        reportError("no href attribute for guide ref");
                    }
                }
                else
                {
                    Href = hrefAttribute.Value;
                    if (getPath != null)
                    {
                        Href = getPath(Href);
                    }
                }
            }
        }

        private void processDocument()
        {
            XDocument doc = GetDocument();

            // Get the root and check if the root has the correct name
            XElement rootElement = doc.Root;
            if (rootElement.Name != XmlNamespace + "ncx")
            {
                reportError("Root element for ncx has an incorrect name: " + rootElement.Name);
            }

            // Parse the nav map
            XElement navMapElement = rootElement.Element(NavMap.ElementName);
            processNavMap(navMapElement);

            // All nav lists
            processNavLists(rootElement);

            XElement guideElement = rootElement.Element(XmlNamespace + "guide");
            processGuide(guideElement);
        }

        private void processNavMap(XElement navMapElement)
        {
            if (navMapElement == null)
            {
                reportError("no NavMap");
                return;
            }

            NavigationMap = new NavMap(navMapElement, reportErrorDelegate, getPathForDelegate);
        }

        private void processNavLists(XElement rootElement)
        {
            IEnumerable<XElement> elements = rootElement.Elements(NavList.ElementName);

            if (elements.Count() < 1)
            {
                // No point creatin the list
                return;
            }

            List<NavList> navigationLists = new List<NavList>();

            foreach (XElement element in elements)
            {
                navigationLists.Add(new NavList(element, reportErrorDelegate, getPathForDelegate));
            }

            NavigationLists = navigationLists;
        }

        private void processGuide(XElement guideElement)
        {
            if (guideElement == null)
            {
                return;
            }

            IEnumerable<XElement> elements = guideElement.Elements(GuideReference.ElementName);
            if (elements.Count() < 1)
            {
                return;
            }

            List<GuideReference> guideReferences = new List<GuideReference>();

            foreach (XElement element in elements)
            {
                guideReferences.Add(new GuideReference(element, reportErrorDelegate, getPathForDelegate));
            }

            GuideReferences = guideReferences;
        }

        private string getPathFor(string path)
        {
            string res = UriResolver.ResolveToString(path);

            if (!IsValidFileName(res))
            {
                reportError("Not a valid path for content: " + res);
                return null;
            }

            return res;
        }

        private void reportError(string msg)
        {
            Log.E(tag, msg);
            HadErrors = true;
        }
    }
}
