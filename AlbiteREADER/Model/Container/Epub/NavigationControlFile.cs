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
        private List<NavList> navigationLists = new List<NavList>();
        public IEnumerable<NavList> NavigationLists
        {
            get { return navigationLists; }
        }

        public NavigationControlFile(IAlbiteContainer container, string filename)
            : base(container, filename)
        {
            XDocument doc = GetDocument();

            // Get the root and make certain the root has the correct name
            XElement rootElement = doc.Element(XmlNamespace + "ncx");

            // Parse the nav map
            XElement navMapElement = rootElement.Element(NavMap.ElementName);
            NavigationMap = new NavMap(this, navMapElement);

            // Parse all nav lists
            foreach (XElement element in rootElement.Elements(NavList.ElementName))
            {
                navigationLists.Add(new NavList(this, element));
            }
        }

        public class NavMap
        {
            public static readonly string ElementName = XmlNamespace + "navMap";

            public NavPoint FirstPoint { get; private set; }

            public NavMap(EpubXmlFile root, XElement element)
            {
                XElement pointElement = element.Element(NavPoint.ElementName);
                if (pointElement != null)
                {
                    FirstPoint = new NavPoint(root, pointElement);
                }
            }
        }

        public class NavPoint : NavContent
        {
            public static readonly string ElementName = XmlNamespace + "navPoint";

            public NavPoint FirstChild { get; private set; }
            public NavPoint NextSibling { get; private set; }

            public NavPoint(EpubXmlFile root, XElement element) : base(root, element)
            {
                XElement child = element.Element(ElementName);
                if (child != null)
                {
                    FirstChild = new NavPoint(root, child);
                }

                IEnumerable<XElement> nextElements = element.ElementsAfterSelf(ElementName);
                if (nextElements.Count() > 0)
                {
                    NextSibling = new NavPoint(root, nextElements.First());
                }
            }
        }

        public class NavList : NavLabel
        {
            public static readonly string ElementName = XmlNamespace + "navList";

            public NavTarget FirstTarget { get; private set; }

            public NavList(EpubXmlFile root, XElement element) : base(element)
            {
                XElement targetElement = element.Element(NavTarget.ElementName);
                if (targetElement != null)
                {
                    FirstTarget = new NavTarget(root, targetElement);
                }
            }
        }

        public class NavTarget : NavContent
        {
            public static readonly string ElementName = XmlNamespace + "navTarget";

            public NavTarget NextSibling { get; protected set; }

            public NavTarget(EpubXmlFile root, XElement element) : base(root, element)
            {
                IEnumerable<XElement> nextElements = element.ElementsAfterSelf(ElementName);
                if (nextElements.Count() > 0)
                {
                    NextSibling = new NavTarget(root, nextElements.First());
                }
            }
        }

        public abstract class NavContent : NavLabel
        {
            private static readonly string elementName = XmlNamespace + "content";
            private static readonly string attributeName = "src";

            public string Src { get; private set; }

            public NavContent(EpubXmlFile root, XElement element) : base(element)
            {
                XElement contentElement = element.Element(elementName);
                XAttribute srcAttribute = contentElement.Attribute(attributeName);

                Src = root.GetUriFor(srcAttribute.Value).ToString();
            }
        }

        public abstract class NavLabel
        {
            private static readonly string labelName = XmlNamespace + "navLabel";
            private static readonly string textName = XmlNamespace + "text";

            public string Label { get; private set; }

            public NavLabel(XElement element)
            {
                XElement labelElement = element.Element(labelName);
                XElement textElement = labelElement.Element(textName);
                Label = textElement.Value;
            }
        }
    }
}
