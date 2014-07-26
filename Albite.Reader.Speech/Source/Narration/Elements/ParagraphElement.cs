using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class ParagraphElement : NarrationElement
    {
        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            Synthesis.Elements.ParagraphElement para = new Synthesis.Elements.ParagraphElement();
            current.AddChild(para);
            return para;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            BreakElement breakEl = new BreakElement(settings.ParagraphAfterPause);
            current.AddChild(breakEl);
            return breakEl;
        }
    }
}
