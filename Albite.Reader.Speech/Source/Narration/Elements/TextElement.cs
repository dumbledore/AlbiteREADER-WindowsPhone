using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public class TextElement : NarrationElement
    {
        public int Id { get; private set; }

        public string Text { get; private set; }

        public object Location { get; private set; }

        public TextElement(int id, string text, object location)
        {
            Id = id;
            Text = text;
            Location = location;
        }

        protected override SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            Synthesis.Elements.TextElement text = new Synthesis.Elements.TextElement(Id, Text, Location);
            current.AddChild(text);
            return text;
        }

        protected override SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            return current.Value;
        }
    }
}
