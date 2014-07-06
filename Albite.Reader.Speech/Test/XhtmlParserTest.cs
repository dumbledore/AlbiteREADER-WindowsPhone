using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;
using Albite.Reader.Speech.Narration.Xhtml;
using System.IO;

namespace Albite.Reader.Speech.Test
{
    public class XhtmlParserTest : TestCase
    {
        public string FilePath { get; private set; }

        public XhtmlParserTest(string filePath)
        {
            FilePath = filePath;
        }

        protected override void TestImplementation()
        {
            using (ResourceStorage res = new ResourceStorage(FilePath))
            {
                using (Stream stream = res.GetStream(FileAccess.Read))
                {
                    using (XhtmlNarrationParser parser = new XhtmlNarrationParser(stream, "en"))
                    {
                        // TODO
                        parser.Parse();
                    }
                }
            }
        }
    }
}
