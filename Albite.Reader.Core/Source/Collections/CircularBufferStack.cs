using System.Collections.Generic;

namespace Albite.Reader.Core.Collections
{
    public class CircularBufferStack<TValue> : CircularBuffer<TValue>
    {
        public CircularBufferStack(int maximumCapacity)
            : base(maximumCapacity) { }

        public CircularBufferStack(IEnumerable<TValue> collection, int maximumCapacity)
            : base(collection, maximumCapacity) { }

        public void Push(TValue item)
        {
            AddHead(item);
        }

        public void Update(TValue item)
        {
            ThrowIfEmpty();
            SetHead(item);
        }

        public TValue Pop()
        {
            return RemoveHead();
        }

        public TValue Peek()
        {
            return GetHead();
        }
    }
}
