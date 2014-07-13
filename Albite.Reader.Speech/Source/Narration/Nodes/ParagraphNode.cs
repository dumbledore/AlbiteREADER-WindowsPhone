namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class ParagraphNode : NarrationNode
    {
        protected override void BuildStart(Builder builder)
        {
            builder.Append("<p>");
        }

        protected override void BuildEnd(Builder builder)
        {
            builder.Append("</p>");
        }
    }
}
