using System.Collections;
using System.Collections.Generic;

namespace Albite.Reader.Core.Collections
{
    public class Tree<TValue> : ITree<TValue>
    {
        public INode<TValue> Root { get; private set; }

        public Tree(INode<TValue> root)
        {
            Root = root;
        }

        public IEnumerator<INode<TValue>> GetEnumerator()
        {
            return new DepthFirstTreeEnumerator<TValue>(Root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
