using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;
using Albite.Reader.Speech.Narration;
using Albite.Reader.Speech.Narration.Xhtml;
using System;
using System.IO;

namespace Albite.Reader.Speech.Test
{
    public class XhtmlNarratorTest : TestCase
    {
        public string FilePath { get; private set; }

        public XhtmlNarratorTest(string filePath)
        {
            FilePath = filePath;
        }

        protected override async void TestImplementation()
        {
            NarrationSettings settings = new NarrationSettings();

            using (ResourceStorage res = new ResourceStorage(FilePath))
            {
                using (Stream stream = res.GetStream(FileAccess.Read))
                {
                    XhtmlNarrator narrator = new XhtmlNarrator(stream, settings);
                    await narrator.ReadAsync();
                }
            }
        }
    }
}
