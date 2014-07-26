using Albite.Reader.Speech.Narration.Elements;
using System.IO;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlNarrator : Narrator<XhtmlLocation>
    {
        public XhtmlNarrator(Stream stream, string baseLanguage, NarrationSettings settings)
            : base(createRoot(stream, baseLanguage, settings), settings)
        {
        }

        private static RootElement createRoot(Stream stream, string baseLanguage, NarrationSettings settings)
        {
            using (XhtmlNarrationParser parser = new XhtmlNarrationParser(stream, baseLanguage, settings))
            {
                return parser.Parse();
            }
        }
    }
}
