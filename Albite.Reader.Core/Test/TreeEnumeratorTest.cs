using Albite.Reader.Core.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Albite.Reader.Core.Test
{
    public class TreeEnumeratorTest : TestCase
    {
        private class Node : INode<string>
        {
            public INode<string> FirstChild { get; private set; }
            public INode<string> NextSibling { get; private set; }
            public int Depth { get; private set; }
            public string Value { get; private set; }

            public Node(Node firstChild, Node nextSibling, int depth, string value)
            {
                FirstChild = firstChild;
                NextSibling = nextSibling;
                Depth = depth;
                Value = value;
            }

            public Node(int depth, string value)
                : this(null, null, depth, value)
            { }
        }

        private class Tree : ITree<string>
        {
            public INode<string> Root { get; private set; }

            public Tree(Node root)
            {
                Root = root;
            }

            public IEnumerator<INode<string>> GetEnumerator()
            {
                return new DepthFirstTreeEnumerator<string>(Root);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        protected override void TestImplementation()
        {
            Tree tree = new Tree(
                new Node(                                   // Root (1)
                    new Node(                               // Root's child (1.1)
                        null,
                        new Node(1, "1.2"),                 // Root's child's sibling (1.2)
                        1, "1.1"),
                    new Node(                               // Root's sibling (2)
                        new Node(                           // Root's sibling's child (2.1)
                            new Node(                       // Root's sibling's child's child (2.1.1)
                                null,
                                new Node(2, "2.1.2"),       // Root's sibling's child's sibling (2.1.2)
                                2, "2.1.1"),
                            null,
                            1, "2.1"),
                        new Node(                           // Root's sibling's sibling (3)
                            new Node(1, "3.1"),             // Root's sibling's sibling's child (3.1)
                            null,
                            0, "3"),
                    0, "2"),
                0, "1")
            );

            // expected order
            List<INode<string>> expectedList = new List<INode<string>>();

            INode<string> node1 = tree.Root;
            expectedList.Add(node1);                                    // 1
            expectedList.Add(node1.FirstChild);                         // 1.1
            expectedList.Add(node1.FirstChild.NextSibling);             // 1.2

            INode<string> node2 = node1.NextSibling;
            expectedList.Add(node2);                                    // 2
            expectedList.Add(node2.FirstChild);                         // 2.1
            expectedList.Add(node2.FirstChild.FirstChild);              // 2.1.1
            expectedList.Add(node2.FirstChild.FirstChild.NextSibling);  // 2.1.2

            INode<string> node3 = node2.NextSibling;
            expectedList.Add(node3);
            expectedList.Add(node3.FirstChild);

            List<INode<string>> resultList = new List<INode<string>>();
            foreach (INode<string> node in tree)
            {
                Log("{1}", node.Value);
                resultList.Add(node);
            }

            if (expectedList.Count != resultList.Count)
            {
                throw new InvalidOperationException("Got a different number of nodes");
            }

            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i] != expectedList[i])
                {
                    throw new InvalidOperationException("Nodes not in expected order");
                }
            }
        }
    }
}
