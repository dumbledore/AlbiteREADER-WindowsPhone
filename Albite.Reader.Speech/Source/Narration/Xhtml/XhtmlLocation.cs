using System.Collections.Generic;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlLocation
    {
        public IEnumerable<int> ElementPath { get; private set; }

        public XhtmlLocation(IEnumerable<int> elementPath)
        {
            ElementPath = elementPath;
        }
    }
}
