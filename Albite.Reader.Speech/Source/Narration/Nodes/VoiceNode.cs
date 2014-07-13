namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class VoiceNode : LanguageNode
    {
        public VoiceNode(string language) : base(language) { }

        protected override void BuildStart(Builder builder)
        {
            builder.Append("<voice xml:lang=\"").Append(Language).Append("\">");
        }

        protected override void BuildEnd(Builder builder)
        {
            builder.Append("</voice>");
        }
    }
}
