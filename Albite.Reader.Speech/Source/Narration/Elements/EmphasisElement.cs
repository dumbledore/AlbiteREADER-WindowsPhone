using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class EmphasisElement : NarrationElement
    {
        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            // Pass. Nothing that tried sounded natural
            return current.Value;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
