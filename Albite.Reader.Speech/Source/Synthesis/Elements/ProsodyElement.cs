namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class ProsodyElement : SynthesisElement
    {
        public float Speed { get; private set; }

        public ProsodyElement(float speed)
        {
            Speed = speed;
        }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<prosody rate=\"").Append(Speed).Append("\">");
        }

        protected override void EndElement(Builder builder)
        {
            builder.Append("</prosody>");
        }
    }
}
