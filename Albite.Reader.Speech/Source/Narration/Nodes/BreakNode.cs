namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class BreakNode : NarrationNode
    {
        public int Duration { get; private set; }

        public BreakNode(int duration)
        {
            Duration = duration;
        }

        protected override void BuildStart(Builder builder)
        {
            builder.Append("<break time=\"").Append(Duration).Append("ms\" />");
        }

        protected override void BuildEnd(Builder builder) { } // Pass
    }
}
