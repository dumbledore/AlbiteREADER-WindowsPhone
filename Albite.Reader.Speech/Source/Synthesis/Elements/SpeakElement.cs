namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class SpeakElement : LanguageElement
    {
        public SpeakElement(string language) : base(language) { }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"");
            builder.Append(Language).Append("\">");
        }

        protected override void EndElement(Builder builder)
        {
            builder.Append("</speak>");
        }
    }
}
