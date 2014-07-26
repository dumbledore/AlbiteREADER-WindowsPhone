using Albite.Reader.Speech.Narration.Elements;
using System.IO;

namespace Albite.Reader.Speech.Narration.Xhtml
{
    public class XhtmlNarrator : Narrator
    {
        public XhtmlNarrator(Stream stream, NarrationSettings settings)
            : base(createRoot(stream, settings), settings)
        {
        }

        private static RootElement createRoot(Stream stream, NarrationSettings settings)
        {
            using (XhtmlNarrationParser parser = new XhtmlNarrationParser(stream, settings))
            {
                return parser.Parse();
            }
        }
    }
}
