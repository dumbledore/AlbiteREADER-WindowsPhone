namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class ProsodyNode : NarrationNode
    {
        public float Speed { get; private set; }

        public ProsodyNode(float speed)
        {
            Speed = speed;
        }

        protected override void BuildStart(Builder builder)
        {
            builder.Append("<prosody rate=\"").Append(Speed).Append("\">");
        }

        protected override void BuildEnd(Builder builder)
        {
            builder.Append("</prosody>");
        }
    }
}
