using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Speech.Synthesis.Elements;
using System;
using Windows.Foundation;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Synthesis
{
    public class Synthesizer : IDisposable
    {
        public SpeakElement Root { get; private set; }

        private SpeechSynthesizer synth;

        public Synthesizer(SpeakElement root)
        {
            Root = root;
            initialiseSynth();
            initialiseTextElements();
        }

        private void initialiseSynth()
        {
            synth = new SpeechSynthesizer();
            synth.BookmarkReached += synth_BookmarkReached;
        }

        private void initialiseTextElements()
        {
            // TODO
        }

        private void synth_BookmarkReached(SpeechSynthesizer sender, SpeechBookmarkReachedEventArgs args)
        {
            int bookmarkId = int.Parse(args.Bookmark);
            // TODO Handle bookmark
        }

        public IAsyncAction ReadAsync()
        {
            //TODO
            string ssml = Root.ToSsml();
            return synth.SpeakSsmlAsync(ssml);
        }

        public void Stop()
        {
            synth.CancelAll();
        }

        public void Dispose()
        {
            synth.Dispose();
        }
    }
}
