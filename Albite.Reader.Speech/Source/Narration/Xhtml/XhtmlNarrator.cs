using Albite.Reader.Speech.Narration.Nodes;
using System.Collections.Generic;
using System.IO;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlNarrator : Narrator<XhtmlLocation>
    {
        public XhtmlNarrator(Stream stream, NarrationSettings settings)
            : base(createRoot(stream, settings), settings)
        {
        }

        private static RootNode createRoot(Stream stream, NarrationSettings settings)
        {
            using (XhtmlNarrationParser parser = new XhtmlNarrationParser(stream, settings))
            {
                return parser.Parse();
            }
        }
    }
}
