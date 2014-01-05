using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Core.Collections
{
    public class CircularBufferStack<TValue> : CircularBuffer<TValue>
    {
        public CircularBufferStack(int maximumCapacity)
            : base(maximumCapacity) { }

        public CircularBufferStack(IEnumerable<TValue> collection)
            : base(collection) { }

        public void Push(TValue item)
        {
            AddHead(item);
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
