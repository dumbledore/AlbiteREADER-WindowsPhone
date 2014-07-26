namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class VoiceElement : LanguageElement
    {
        public VoiceElement(string language) : base(language) { }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<voice xml:lang=\"").Append(Language).Append("\">");
        }

        protected override void EndElement(Builder builder)
        {
            builder.Append("</voice>");
        }
    }
}