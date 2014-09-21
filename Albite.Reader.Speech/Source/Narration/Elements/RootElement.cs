using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class RootElement : LanguageElement
    {
        public RootElement(string language) : base(language) { }

        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            // Use the provided language instead of base language here
            SynthesisElement root = new SpeakElement(Language);
            current.AddChild(root);

            // Took base voice's gender into account
            SynthesisElement voice = new VoiceElement(Language, settings.BaseVoice.Male);
            root.AddChild(voice);

            // And now take care of base narration speed
            SynthesisElement prosody = new ProsodyElement(settings.BaseSpeedRatio);
            voice.AddChild(prosody);

            return prosody;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
