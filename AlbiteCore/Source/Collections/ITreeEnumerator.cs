using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Core.Collections
{
    public interface ITreeEnumerator<TValue> : IEnumerator<INode<TValue>>
    {
    }
}
