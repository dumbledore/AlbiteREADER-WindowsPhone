using System;
using System.Collections.Generic;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlLocation
    {
        private int[] elementPath;
        public IList<int> ElementPath
        {
            get { return Array.AsReadOnly<int>(elementPath); }
        }

        public int TextOffset { get; private set; }

        public XhtmlLocation(int[] elementPath, int textOffset)
        {
            this.elementPath = elementPath;
            TextOffset = textOffset;
        }

        public override string ToString()
        {
            string p = string.Join<int>(", ", elementPath);
            return "Location: {{ Node: [ " + p + " ], TextOffset: " + TextOffset + " }}";
        }
    }
}
