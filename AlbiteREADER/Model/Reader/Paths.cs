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

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public static class Paths
    {
        public static string BasePath
        {
            get { return "BrowserEngine"; }
        }

        public static string BaseStyles
        {
            get { return "Base.css"; }
        }

        public static string ContentStyles
        {
            get { return "Content.css"; }
        }

        public static string JSEngine
        {
            get { return "Albite.js"; }
        }

        public static string MainPage
        {
            get { return "Main.xhtml"; }
        }

        public static string ThemeStyles
        {
            get { return "Theme.css"; }
        }
    }
}
