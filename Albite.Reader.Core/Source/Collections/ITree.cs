using System.Collections;
using System.Collections.Generic;
namespace Albite.Reader.Core.Collections
{
    public interface ITree<TValue>: IEnumerable<INode<TValue>>, IEnumerable
    {
        INode<TValue> Root { get; }
    }
}
