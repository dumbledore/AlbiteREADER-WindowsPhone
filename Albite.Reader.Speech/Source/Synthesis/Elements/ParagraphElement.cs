namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class ParagraphElement : SynthesisElement
    {
        protected override void StartElement(Builder builder)
        {
            builder.Append("<p>");
        }

        protected override void EndElement(Builder builder)
        {
            builder.Append("</p>");
        }
    }
}
