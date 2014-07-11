using Albite.Reader.Speech.Narration.Commands;
using System.Collections.Generic;
using System.IO;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlNarrator : Narrator<XhtmlNarrationExpression>
    {
        public XhtmlNarrator(Stream stream, NarrationSettings settings)
            : base(createRoot(stream, settings), settings)
        {
        }

        private static NarrationCommand createRoot(Stream stream, NarrationSettings settings)
        {
            using (XhtmlNarrationParser parser = new XhtmlNarrationParser(stream, settings))
            {
                return parser.Parse();
            }
        }
    }
}
