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
        public SpeakElement Root { get; private set; }

        private SpeechSynthesizer synth;

        public LocatedTextManager<TLocation> LocatedTextManager { get; private set; }

        private LocatedManagerUpdaterProxy LocalManagerUpdaterProxy;

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

            LocalManagerUpdaterProxy = new LocatedManagerUpdaterProxy();
            LocatedTextManager = new LocatedTextManager<TLocation>(list.ToArray(), LocalManagerUpdaterProxy);
        }

        private int currentTextElement = 0;
        private object currentTextElementLock = new object();

        private void synth_BookmarkReached(SpeechSynthesizer sender, SpeechBookmarkReachedEventArgs args)
        {
            int bookmarkId = int.Parse(args.Bookmark);
            // Sometimes, if there are two bookmarks very close together, they might come
            // in order different from the one in the SSML. I suppose this is because each
            // event might be running on a different thread (probably from the thread pool).
            // So we need to sync this, otherwise it might appear we are going backwards.
            bool notify = false;

            lock (currentTextElementLock)
            {
                if (bookmarkId > currentTextElement)
                {
                    currentTextElement = bookmarkId;
                    notify = true;
                }
            }

            if (notify)
            {
                LocalManagerUpdaterProxy.Updater.Update(bookmarkId);
            }
        }

        private class LocatedManagerUpdaterProxy
            : LocatedTextManager<TLocation>.ILocatedTextManagerUpdaterProxy
        {
            public LocatedTextManager<TLocation>.ILocatedTextManagerUpdater Updater { get; private set; }

            public void SetUpdater(LocatedTextManager<TLocation>.ILocatedTextManagerUpdater updater)
            {
                Updater = updater;
            }
        }

        public IAsyncAction ReadAsync()
        {
            string ssml = Root.ToSsml(LocatedTextManager.Current.Id);
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
