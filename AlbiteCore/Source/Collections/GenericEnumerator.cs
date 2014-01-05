using System;
using System.Collections;
using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Core.Collections
{
    public class GenericEnumerator<TValue> : IEnumerator<TValue>
    {
        private IEnumerator enumerator;

        public GenericEnumerator(IEnumerable enumerable)
        {
            enumerator = enumerable.GetEnumerator();
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        void IDisposable.Dispose() { }

        public TValue Current
        {
            get { return (TValue)enumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return enumerator.Current; }
        }
    }
}
