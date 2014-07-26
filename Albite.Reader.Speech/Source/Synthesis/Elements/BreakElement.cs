namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class BreakElement : SynthesisElement
    {
        public int Duration { get; private set; }

        public BreakElement(int duration)
        {
            Duration = duration;
        }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<break time=\"").Append(Duration).Append("ms\" />");
        }

        protected override void EndElement(Builder builder) { } // Pass
    }
}
