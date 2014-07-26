using Albite.Reader.Core.Collections;
using Albite.Reader.Speech.Synthesis.Elements;

namespace Albite.Reader.Speech.Narration.Elements
{
    public abstract class NarrationElement : AbstractNode<NarrationElement>
    {
        protected abstract SynthesisElement StartElement(NarrationSettings settings, AbstractNode<SynthesisElement> current);
        protected abstract SynthesisElement EndElement(NarrationSettings settings, AbstractNode<SynthesisElement> current);

        public override NarrationElement Value
        {
            get { return this; }
        }


        public SynthesisElement ToSynthesisElement(NarrationSettings settings)
        {
            DummyNode dummy = new DummyNode();
            build(settings, dummy);
            return dummy.FirstChild.Value;
        }

        private void build(NarrationSettings settings, AbstractNode<SynthesisElement> current)
        {
            INode<NarrationElement> node = this;

            while (node != null)
            {
                SynthesisElement el = node.Value.StartElement(settings, current);

                if (node.FirstChild != null)
                {
                    // Build children contents
                    node.FirstChild.Value.build(settings, el);
                }

                node.Value.EndElement(settings, current);

                // Next node
                node = node.NextSibling;
            }
        }

        private class DummyNode : AbstractNode<SynthesisElement>
        {
            public override SynthesisElement Value
            {
                get { return null; }
            }
        }
    }
}
