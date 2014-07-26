using Albite.Reader.Core.Collections;
using System.Text;

namespace Albite.Reader.Speech.Synthesis.Elements
{
    public abstract class SynthesisElement : AbstractNode<SynthesisElement>
    {
        protected abstract void StartElement(Builder builder);
        protected abstract void EndElement(Builder builder);

        public override SynthesisElement Value
        {
            get { return this; }
        }

        public string ToSsml(int firstTextElementId = 0)
        {
            Builder b = new Builder();
            build(b, firstTextElementId);
            return b.ToString();
        }

        private bool build(Builder builder, int firstTextElementId, bool reachedText = false)
        {
            INode<SynthesisElement> node = this;

            while (node != null)
            {
                if (node.Value is TextElement)
                {
                    TextElement text = (TextElement)node.Value;
                    reachedText = text.Id >= firstTextElementId;
                }

                bool buildNode = reachedText || !(node.Value is TextElement || node.Value is BreakElement);

                if (buildNode)
                {
                    // Start tag
                    node.Value.StartElement(builder);
                }

                if (node.FirstChild != null)
                {
                    // Build children contents
                    node.FirstChild.Value.build(builder, firstTextElementId, reachedText);
                }

                if (buildNode)
                {
                    // End tag
                    node.Value.EndElement(builder);
                }

                // Next node
                node = node.NextSibling;
            }

            return reachedText;
        }

        protected class Builder
        {
            private StringBuilder b = new StringBuilder();

            public Builder Append(string s)
            {
                b.Append(s);
                return this;
            }

            public Builder Append(float f)
            {
                b.Append(f);
                return this;
            }

            public Builder Append(int i)
            {
                b.Append(i);
                return this;
            }

            public Builder Append(object o)
            {
                b.Append(o);
                return this;
            }

            public override string ToString()
            {
                return b.ToString();
            }
        }
    }
}
