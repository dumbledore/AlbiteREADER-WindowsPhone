using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class TextElement<TLocation> : NarrationElement
    {
        public int Id { get; private set; }

        public string Text { get; private set; }

        public TLocation Location { get; private set; }

        public TextElement(int id, string text, TLocation location)
        {
            Id = id;
            Text = text;
            Location = location;
        }

        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            LocatedTextElement<TLocation> text = new LocatedTextElement<TLocation>(Id, Text, Location);
            current.AddChild(text);
            return text;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
