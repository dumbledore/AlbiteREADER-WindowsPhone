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
    public class DomLocation
    {
        /// <summary>
        /// This is the Elementh Path
        /// </summary>
        public string ElementPath { get; private set; }

        /// <summary>
        /// This is the offset in the text so that
        /// the correct line would be selected.
        /// </summary>
        public int TextOffset { get; private set; }

        public DomLocation(string path, int offset)
        {
            ElementPath = path;
            TextOffset = offset;
        }
    }
}
