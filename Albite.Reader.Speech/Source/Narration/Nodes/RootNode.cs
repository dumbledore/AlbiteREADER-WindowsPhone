namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class RootNode : LanguageNode
    {
        public RootNode(string language) : base(language) { }

        protected override void BuildStart(Builder builder)
        {
            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"");
            builder.Append(Language).Append("\">");
        }

        protected override void BuildEnd(Builder builder)
        {
            builder.Append("</speak>");
        }
    }
}
