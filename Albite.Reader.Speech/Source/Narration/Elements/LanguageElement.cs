using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class LanguageElement : NarrationElement
    {
        public string Language { get; private set; }

        public LanguageElement(string language)
        {
            Language = language;
        }

        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            VoiceElement voice = new VoiceElement(Language, settings.BaseVoice.Male);
            current.AddChild(voice);
            return voice;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
