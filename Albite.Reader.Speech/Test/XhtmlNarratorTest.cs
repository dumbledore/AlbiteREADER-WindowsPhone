using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;
using Albite.Reader.Speech.Narration;
using Albite.Reader.Speech.Narration.Xhtml;
using System;
using System.IO;
using System.Linq;
using Windows.Foundation;

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
            IAsyncAction a;

            using (ResourceStorage res = new ResourceStorage(FilePath))
            {
                using (Stream stream = res.GetStream(FileAccess.Read))
                {
                    using (XhtmlNarrator narrator = new XhtmlNarrator(stream, "en", settings))
                    {

                        narrator.LocatedTextManager.TextReached += LocatedTextManager_TextReached;

                        Log("Reading 10s of text");
                        a = narrator.ReadAsync();
                        System.Threading.Thread.Sleep(10000);
                        narrator.Stop();
                        Log("Enough");

                        Log("Now restart the read. Should continue from the last text");
                        a = narrator.ReadAsync();
                        System.Threading.Thread.Sleep(5000);
                        narrator.Stop();
                        Log("Stop the narrator");

                        Log("Now skip some nodes and read from a later position");
                        narrator.LocatedTextManager.Current = narrator.LocatedTextManager.Locations.Skip(88).First();

                        // Await completion
                        await narrator.ReadAsync();
                        Log("Narration ended");
                    }
                }
            }
        }

        void LocatedTextManager_TextReached(LocatedTextManager<XhtmlLocation> sender, ILocatedText<XhtmlLocation> args)
        {
            Log("Reached #{0}: {1}", args.Id, args.Text);
        }
    }
}
