using System.Collections.Generic;
using System.IO;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlbNarrator : Narrator<XhtmlNarrationExpression>
    {
        public XhtmlbNarrator(Stream stream, NarrationSettings settings)
            : base(createRoot(stream, settings))
        {
        }

        private static INarrationCommand createRoot(Stream stream, NarrationSettings settings)
        {
            using (XhtmlNarrationParser parser = new XhtmlNarrationParser(stream, settings.BaseLanguage))
            {
                return parser.Parse();
            }
        }
    }
}
