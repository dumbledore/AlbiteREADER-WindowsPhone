using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Synthesis
{
    public class Synthesizer<TLocation> : IDisposable
    {
        public event TypedEventHandler<Synthesizer<TLocation>, ILocatedText<TLocation>> TextReached;

        public SpeakElement Root { get; private set; }

        private SpeechSynthesizer synth;

        private LocatedTextElement<TLocation>[] textElements;

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
            List<LocatedTextElement<TLocation>> list = new List<LocatedTextElement<TLocation>>();

            Tree<SynthesisElement> tree = new Tree<SynthesisElement>(Root);
            foreach (SynthesisElement el in tree)
            {
                if (el is LocatedTextElement<TLocation>)
                {
                    list.Add((LocatedTextElement<TLocation>)el);
                }
            }

            textElements = list.ToArray();
        }

        private int currentTextElement = 0;

        private void synth_BookmarkReached(SpeechSynthesizer sender, SpeechBookmarkReachedEventArgs args)
        {
            int bookmarkId = int.Parse(args.Bookmark);
            // Sometimes, if there are two bookmarks very close together, they might come
            // in order different from the one in the SSML. I suppose this is because each
            // event might be running on a different thread (probably from the thread pool).
            // So we need to sync this, otherwise it might appear we are going backwards.
            bool notify = false;

            lock (textElements)
            {
                if (bookmarkId > currentTextElement)
                {
                    currentTextElement = bookmarkId;
                    notify = true;
                }
            }

            if (notify)
            {
                LocatedTextElement<TLocation> text = textElements[bookmarkId];
                if (TextReached != null)
                {
                    TextReached(this, text);
                }
            }
        }

        public IAsyncAction ReadAsync()
        {
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
