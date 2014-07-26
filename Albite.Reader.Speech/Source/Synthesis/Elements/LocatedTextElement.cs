namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class LocatedTextElement<TLocation> : TextElement, ILocatedText<TLocation>
    {
        public TLocation Location { get; private set; }

        public LocatedTextElement(int id, string text, TLocation location)
            : base(id, text)
        {
            Location = location;
        }

        private LocatedTextElement() { }

        // Used for copying the element so that the "node" info would be reset
        public LocatedTextElement<TLocation> CopyToNew()
        {
            LocatedTextElement<TLocation> n = new LocatedTextElement<TLocation>();
            n.Id = Id;
            n.Text = Text;
            n.Location = Location;
            return n;
        }
    }
}
