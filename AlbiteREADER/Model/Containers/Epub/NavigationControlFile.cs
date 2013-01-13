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
using System.Xml.Linq;

namespace SvetlinAnkov.AlbiteREADER.Model.Containers.Epub
{
    public class NavigationControlFile
    {
        public TableOfContents TableOfContents { get; private set; }

        public NavigationControlFile(XDocument doc)
        {
        }
    }
}
