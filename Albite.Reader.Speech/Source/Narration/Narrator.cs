using Albite.Reader.Speech.Narration.Elements;
using Albite.Reader.Speech.Synthesis;
using Albite.Reader.Speech.Synthesis.Elements;
using Windows.Foundation;

namespace Albite.Reader.Speech.Narration
{
    public abstract class Narrator
    {
        public RootElement Root { get; private set; }

        private Synthesizer synth;

        protected Narrator(RootElement root, NarrationSettings settings)
        {
            Root = root;
            SynthesisElement sRoot = Root.ToSynthesisElement(settings);
            synth = new Synthesizer((SpeakElement)sRoot);
        }

        public IAsyncAction ReadAsync()
        {
            return synth.ReadAsync();
        }

        public void Stop()
        {
            synth.Stop();
        }
    }
}
