namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class VoiceElement : LanguageElement
    {
        public VoiceElement(string language, bool male)
            : base(language)
        {
            Male = male;
        }

        public bool Male { get; private set; }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<voice");
            builder.Append(" gender=\"").Append(Male ? "male" : "female").Append("\"");
            builder.Append(" xml:lang=\"").Append(Language).Append("\">");
        }

        protected override void EndElement(Builder builder)
        {
            builder.Append("</voice>");
        }
    }
}