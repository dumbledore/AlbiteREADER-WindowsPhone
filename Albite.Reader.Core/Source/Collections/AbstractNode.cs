namespace Albite.Reader.Core.Collections
{
    public abstract class AbstractNode<TValue> : INode<TValue>
    {
        private AbstractNode<TValue> firstChild_;
        public INode<TValue> FirstChild
        {
            get { return firstChild_; }
        }

        private AbstractNode<TValue> lastChild_;
        public INode<TValue> LastChild
        {
            get { return lastChild_; }
        }

        private AbstractNode<TValue> nextSibling_;
        public INode<TValue> NextSibling
        {
            get { return nextSibling_; }
        }

        public abstract TValue Value { get;}

        public void AddChild(AbstractNode<TValue> newChild)
        {
            if (firstChild_ == null)
            {
                // FirstChild and LastChild can both be null
                firstChild_ = newChild;
            }
            else
            {
                // or both be not null
                lastChild_.nextSibling_ = newChild;
            }

            lastChild_ = newChild;
        }
    }
}
