using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class EmphasisElement : NarrationElement
    {
        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            BreakElement breakEl = new BreakElement(settings.EmphasisPause);
            current.AddChild(breakEl);

            ProsodyElement prosody = new ProsodyElement(settings.EmphasisSpeedRatio, settings.EmphasisPitch);
            breakEl.AddChild(prosody);
            return prosody;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
