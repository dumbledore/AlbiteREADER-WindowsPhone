using Albite.Reader.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Phone.Speech.Synthesis;
/*
namespace Albite.Reader.Speech.Narration
{
    internal class NarrationEngineSynthesizer<TExpression> : NarrationEngine<TExpression>, IDisposable where TExpression : NarrationExpression
    {
        private static readonly string Tag = "NarrationEngineSynthesizer";

        private Dictionary<string, VoiceInformation> voices = new Dictionary<string, VoiceInformation>();

        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        public NarrationEngineSynthesizer(NarrationState<TExpression> state, CancellationToken token)
            : base(state, token)
        {
        }

        public override void Read(string text)
        {
            Log.D(Tag, "Reading " + text);

            // TODO: Cache the voice and update via callback form INarrationState.Langauge
            synthesizer.SetVoice(getVoice(State.Language));
            synthesizer.SpeakTextAsync(text).AsTask().Wait(Token);
        }

        private VoiceInformation getVoice(string language)
        {
            VoiceInformation voice = null;

            if (!voices.TryGetValue(language, out voice))
            {
                // Not cached
                voice = findVoice(language);
            }

            return voice;
        }

        private VoiceInformation findVoice(string language)
        {
            foreach (VoiceInformation voice in InstalledVoices.All)
            {
                if (voice.Language.Equals(language, StringComparison.InvariantCultureIgnoreCase))
                {
                    return voice;
                }
            }

            return InstalledVoices.Default;
        }

        public void Dispose()
        {
            synthesizer.Dispose();
        }
    }
}
*/
