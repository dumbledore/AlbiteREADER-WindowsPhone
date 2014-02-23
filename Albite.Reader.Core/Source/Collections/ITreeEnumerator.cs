using System.Collections.Generic;

namespace Albite.Reader.Core.Collections
{
    public interface ITreeEnumerator<TValue> : IEnumerator<INode<TValue>>
    {
    }
}
