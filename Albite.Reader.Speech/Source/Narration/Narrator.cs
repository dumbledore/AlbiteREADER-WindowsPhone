using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Speech.Narration.Elements;
using Albite.Reader.Speech.Synthesis;
using Albite.Reader.Speech.Synthesis.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Narration
{
    public abstract class Narrator
    {
        private static readonly string Tag = "Narrator";

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
