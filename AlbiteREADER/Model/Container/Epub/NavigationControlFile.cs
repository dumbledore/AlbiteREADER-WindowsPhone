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
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Model.Container.Epub
{
    /// <summary>
    /// Check http://idpf.org/epub/20/spec/OPF_2.0.1_draft.htm#Section2.4.1
    /// </summary>
    public class NavigationControlFile : EpubXmlFile
    {
        public static string XmlNamespace { get { return "{http://www.daisy.org/z3986/2005/ncx/}"; } }

        public NavMap NavigationMap { get; private set; }

        public IEnumerable<NavList> NavigationLists { get; private set; }

        /// <summary>
        /// Returns true if there was a problem with parsing the file.
        /// </summary>
        public bool HadErrors { get; private set; }

        private static readonly string tag = "NavigationControlFile";

        private NavObject.GetUriForDelegate getUriForDelegate;
        private NavObject.ReportErrorDelegate reportErrorDelegate;

        public NavigationControlFile(IAlbiteContainer container, string filename)
            : base(container, filename)
        {
            getUriForDelegate = new NavObject.GetUriForDelegate(GetUriForToString);
            reportErrorDelegate = new NavObject.ReportErrorDelegate(reportError);
            HadErrors = false;
            processDocument();
        }

        public class NavMap : NavObject
        {
            public static readonly string ElementName = XmlNamespace + "navMap";

            public NavPoint FirstPoint { get; private set; }

            public NavMap(XElement element, ReportErrorDelegate reportError, GetUriForDelegate getUri)
            {
                XElement pointElement = element.Element(NavPoint.ElementName);
                if (pointElement != null)
                {
                    FirstPoint = new NavPoint(pointElement, reportError, getUri);
                }
            }
        }

        public class NavPoint : NavContent
        {
            public static readonly string ElementName = XmlNamespace + "navPoint";

            public NavPoint FirstChild { get; private set; }
            public NavPoint NextSibling { get; private set; }

            public NavPoint(XElement element, ReportErrorDelegate reportError, GetUriForDelegate getUri)
                : base(element, reportError, getUri)
            {
                XElement child = element.Element(ElementName);
                if (child != null)
                {
                    FirstChild = new NavPoint(child, reportError, getUri);
                }

                IEnumerable<XElement> nextElements = element.ElementsAfterSelf(ElementName);
                if (nextElements.Count() > 0)
                {
                    NextSibling = new NavPoint(nextElements.First(), reportError, getUri);
                }
            }
        }

        public class NavList : NavLabel
        {
            public static readonly string ElementName = XmlNamespace + "navList";

            public NavTarget FirstTarget { get; private set; }

            public NavList(XElement element, ReportErrorDelegate reportError, GetUriForDelegate getUri)
                : base(element, reportError)
            {
                XElement targetElement = element.Element(NavTarget.ElementName);
                if (targetElement != null)
                {
                    FirstTarget = new NavTarget(targetElement, reportError, getUri);
                }
            }
        }

        public class NavTarget : NavContent
        {
            public static readonly string ElementName = XmlNamespace + "navTarget";

            public NavTarget NextSibling { get; protected set; }

            public NavTarget(XElement element, ReportErrorDelegate reportError, GetUriForDelegate getUri)
                : base(element, reportError, getUri)
            {
                IEnumerable<XElement> nextElements = element.ElementsAfterSelf(ElementName);
                if (nextElements.Count() > 0)
                {
                    NextSibling = new NavTarget(nextElements.First(), reportError, getUri);
                }
            }
        }

        public abstract class NavContent : NavLabel
        {
            private static readonly string elementName = XmlNamespace + "content";
            private static readonly string attributeName = "src";

            public string Src { get; private set; }

            public NavContent(XElement element, ReportErrorDelegate reportError, GetUriForDelegate getUri)
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

                // If the GetUri delegate is available, pass the path through it.
                // It doesn't look perfectly readable this way, but it's
                // the way to get our abstraction.
                if (getUri != null)
                {
                    Src = getUri(Src);
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
            public delegate string GetUriForDelegate(string path);
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
        }

        private void processNavMap(XElement navMapElement)
        {
            if (navMapElement == null)
            {
                reportError("no NavMap");
                return;
            }

            NavigationMap = new NavMap(navMapElement, reportErrorDelegate, getUriForDelegate);
        }

        private void processNavLists(XElement rootElement)
        {
            List<NavList> navigationLists = new List<NavList>();

            foreach (XElement element in rootElement.Elements(NavList.ElementName))
            {
                navigationLists.Add(new NavList(element, reportErrorDelegate, getUriForDelegate));
            }

            NavigationLists = navigationLists;
        }

        private void reportError(string msg)
        {
            Log.E(tag, msg);
            HadErrors = true;
        }

        private string GetUriForToString(string path)
        {
            return GetUriFor(path).ToString();
        }
    }
}
