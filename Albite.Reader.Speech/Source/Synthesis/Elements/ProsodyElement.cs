namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class ProsodyElement : SynthesisElement
    {
        public float Speed { get; private set; }

        public int Pitch { get; private set; }

        public ProsodyElement(float speed, int pitch = 0)
        {
            Speed = speed;
            Pitch = pitch;
        }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<prosody rate=\"").Append(Speed).Append("\" ");
            if (Pitch != 0)
            {
                builder.Append("pitch=\"").Append(Pitch).Append("st\" ");
            }
            builder.Append(">");
        }

        protected override void EndElement(Builder builder)
        {
            builder.Append("</prosody>");
        }
    }
}
