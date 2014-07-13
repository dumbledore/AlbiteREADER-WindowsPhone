using Albite.Reader.Speech.Narration.Xhtml;

namespace Albite.Reader.Speech.Narration.Nodes
{
    internal class TextNode : NarrationNode
    {
        public int Id { get; private set; }

        public string Text { get; private set; }

        public TextNode(int id, string text)
        {
            Id = id;
            Text = adjustText(text);
        }

        private static string adjustText(string text)
        {
            return text.Replace("\n", "");
        }

        protected override void BuildStart(Builder builder)
        {
            builder.Append(Text);
        }

        protected override void BuildEnd(Builder builder) { } // Pass
    }
}
