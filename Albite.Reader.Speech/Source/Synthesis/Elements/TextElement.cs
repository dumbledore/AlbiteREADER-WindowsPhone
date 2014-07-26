namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class TextElement : SynthesisElement
    {
        public int Id { get; private set; }

        public string Text { get; private set; }

        public object Location { get; private set; }

        public TextElement(int id, string text, object location)
        {
            Id = id;
            Text = adjustText(text);
            Location = location;
        }

        private static string adjustText(string text)
        {
            return text.Replace("\n", " ");
        }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<mark name=\"").Append(Id).Append("\" />");
            builder.Append(Text);
        }

        protected override void EndElement(Builder builder) { } // Pass
    }
}
