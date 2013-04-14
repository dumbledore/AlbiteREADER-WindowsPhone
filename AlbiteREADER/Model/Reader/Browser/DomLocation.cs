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

namespace SvetlinAnkov.Albite.READER.Model.Reader.Browser
{
    public class DomLocation
    {
        /// <summary>
        /// This is the Unique element ID, got using the related IE API.
        /// To serialize:
        ///     ElementIndex = this.sourceIndex
        ///
        /// To deserialize:
        ///     document.getElementsByTagName('*')[ElementIndex]
        ///
        /// Or Even better:
        ///     document.all[ElementIndex]
        /// </summary>
        public int ElementIndex { get; private set; }

        /// <summary>
        /// If the element was of type Text, this is the offset in
        /// the text so that the correct line would be selected.
        ///
        /// If it was not a Text element, TextOffset should be -1.
        /// </summary>
        public int TextOffset { get; private set; }

        public DomLocation(int index, int offset)
        {
            ElementIndex = index;
            TextOffset = offset;
        }
    }
}
