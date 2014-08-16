using Albite.Reader.Speech.Narration.Elements;
using Albite.Reader.Speech.Synthesis;
using Albite.Reader.Speech.Synthesis.Elements;
using System;
using Windows.Foundation;

namespace Albite.Reader.Speech.Narration
{
    public abstract class Narrator<TLocation> : IDisposable
    {
        public RootElement Root { get; private set; }

        private Synthesizer<TLocation> synth;

        public LocatedTextManager<TLocation> LocatedTextManager
        {
            get { return synth.LocatedTextManager; }
        }

        protected Narrator(RootElement root, NarrationSettings settings)
        {
            Root = root;
            SynthesisElement sRoot = Root.ToSynthesisElement(settings);
            synth = new Synthesizer<TLocation>((SpeakElement)sRoot);
        }

        public IAsyncAction ReadAsync()
        {
            return synth.ReadAsync();
        }

        public void Stop()
        {
            synth.Stop();
        }

        public void Dispose()
        {
            synth.Dispose();
        }
    }
}
