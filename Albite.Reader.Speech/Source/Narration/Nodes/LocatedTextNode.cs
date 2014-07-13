namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class LocatedTextNode<TLocation> : TextNode
    {
        public TLocation Location { get; private set; }

        public LocatedTextNode(int id, string text, TLocation location)
            : base(id, text)
        {
            Location = location;
        }
    }
}
