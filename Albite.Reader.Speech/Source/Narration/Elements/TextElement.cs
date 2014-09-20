using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class TextElement<TLocation> : NarrationElement, ILocatedText<TLocation>
    {
        private LocatedTextElement<TLocation> cachedText;

        public int Id
        {
            get { return cachedText.Id; }
        }

        public string Text
        {
            get { return cachedText.Text; }
        }

        public TLocation Location
        {
            get { return cachedText.Location; }
        }

        public TextElement(int id, string text, TLocation location)
        {
            cachedText = new LocatedTextElement<TLocation>(id, text, location);
        }

        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            current.AddChild(cachedText.CopyToNew());
            return cachedText;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
