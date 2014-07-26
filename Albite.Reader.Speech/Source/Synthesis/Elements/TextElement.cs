using System.Text;

namespace Albite.Reader.Speech.Synthesis.Elements
{
    public class TextElement : SynthesisElement
    {
        public int Id { get; protected set; }

        public string Text { get; protected set; }

        protected TextElement() { }

        public TextElement(int id, string text)
        {
            Id = id;
            Text = adjustText(text);
        }

        private static char[] charsToRemove = new char[]
        {
            '\n',
            '\r',
            '*',
        };

        private static string adjustText(string text)
        {
            StringBuilder b = new System.Text.StringBuilder(text);
            foreach (char c in charsToRemove)
            {
                b.Replace(c, ' ');
            }
            return b.ToString();
        }

        protected override void StartElement(Builder builder)
        {
            builder.Append("<mark name=\"").Append(Id).Append("\" />");
            builder.Append(Text);
        }

        protected override void EndElement(Builder builder) { } // Pass
    }
}
