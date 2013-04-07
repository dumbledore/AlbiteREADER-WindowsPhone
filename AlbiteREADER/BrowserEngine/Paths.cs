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

namespace SvetlinAnkov.Albite.READER.BrowserEngine
{
    public static class Paths
    {
        public static string BaseStyles
        {
            get { return "BrowserEngine/Base.css"; }
        }

        public static string ContentStyles
        {
            get { return "BrowserEngine/Content.css"; }
        }

        public static string JSEngine
        {
            get { return "BrowserEngine/Albite.js"; }
        }

        public static string MainPage
        {
            get { return "BrowserEngine/Main.xhtml"; }
        }

        public static string ThemeStyles
        {
            get { return "Layout/Theme.css"; }
        }
    }
}
