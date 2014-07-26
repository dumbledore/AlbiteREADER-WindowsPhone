using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class RootElement : LanguageElement
    {
        public RootElement(string language) : base(language) { }

        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            SynthesisElement root = new SpeakElement(Language);
            current.AddChild(root);

            SynthesisElement prosody = new ProsodyElement(settings.BaseSpeedRatio);
            root.AddChild(prosody);

            return prosody;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
