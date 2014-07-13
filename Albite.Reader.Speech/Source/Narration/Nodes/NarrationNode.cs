using System.Text;

namespace Albite.Reader.Speech.Narration.Nodes
{
    internal abstract class NarrationNode
    {
        public NarrationNode FirstChild { get; private set; }
        public NarrationNode LastChild { get; private set; }
        public NarrationNode Next { get; private set; }

        public bool Enabled { get; set; }

        public NarrationNode()
        {
            Enabled = true;
        }

        public void AddChild(NarrationNode newChild)
        {
            if (FirstChild == null)
            {
                // FirstChild and LastChild can both be null
                FirstChild = newChild;
            }
            else
            {
                // or both be not null
                LastChild.Next = newChild;
            }

            LastChild = newChild;
        }

        public string ToSsml()
        {
            Builder b = new Builder();
            build(b);
            return b.ToString();
        }

        private void build(Builder builder)
        {
            NarrationNode node = this;

            while (node != null)
            {
                // Start tag
                node.BuildStart(builder);

                if (node.FirstChild != null)
                {
                    // Build children contents
                    node.FirstChild.build(builder);
                }

                // End tag
                node.BuildEnd(builder);

                // Next node
                node = node.Next;
            }
        }

        protected abstract void BuildStart(Builder builder);
        protected abstract void BuildEnd(Builder builder);

        public class Builder
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
