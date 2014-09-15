using System;

namespace Albite.Reader.Core.Collections
{
    public abstract class AbstractNode<TValue> : INode<TValue>
    {
        public abstract TValue Value { get; }

        private AbstractNode<TValue> firstChild_;
        public INode<TValue> FirstChild
        {
            get { return firstChild_; }
        }

        public INode<TValue> LastChild
        {
            get { return lastChild_; }
        }

        private AbstractNode<TValue> lastChild_
        {
            get
            {
                if (firstChild_ != null)
                {
                    if (firstChild_.nextSibling_ != null)
                    {
                        return firstChild_.lastSibling_;
                    }

                    // no siblings, so return the first child directly
                    return firstChild_;
                }
                return null;
            }
        }

        private AbstractNode<TValue> nextSibling_;
        public INode<TValue> NextSibling
        {
            get { return nextSibling_; }
        }

        public INode<TValue> LastSibling
        {
            get { return lastSibling_; }
        }

        private AbstractNode<TValue> lastSibling_
        {
            get
            {
                AbstractNode<TValue> sibling = nextSibling_;
                if (sibling != null)
                {
                    while (sibling.nextSibling_ != null)
                    {
                        sibling = sibling.nextSibling_;
                    }
                }
                return sibling;
            }
        }

        public void AddChild(AbstractNode<TValue> newChild)
        {
            newChild.makeSureNotAdded();

            if (firstChild_ == null)
            {
                firstChild_ = newChild;
            }
            else
            {
                lastChild_.nextSibling_ = newChild;
            }
        }

        public void AddSibling(AbstractNode<TValue> newSibling)
        {
            newSibling.makeSureNotAdded();

            if (nextSibling_ != null)
            {
                newSibling.nextSibling_ = nextSibling_;
            }

            nextSibling_ = newSibling;
        }

        // This check is done so that the same node is not added
        // in different trees. Note that the node can still be empty,
        // i.e. firstChild/firstSibling being null, yet still can
        // be a leaf for several trees. That would be a logical
        // issue, so to keep things simple and static, the node
        // can be added only once. Of course, one can add
        // root/previousSibling and get a full-blown tree
        // yet, moving nodes around would make things complex,
        // which is not needed at the moment, i.e. static
        // trees are quite fine for now.
        private void makeSureNotAdded()
        {
            if (alreadyAdded)
            {
                throw new InvalidOperationException("Node already added");
            }

            alreadyAdded = true;
        }

        private bool alreadyAdded = false;
    }
}
