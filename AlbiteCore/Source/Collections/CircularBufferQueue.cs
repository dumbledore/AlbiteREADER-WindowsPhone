using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Core.Collections
{
    public class CircularBufferQueue<TValue> : CircularBuffer<TValue>
    {
        public CircularBufferQueue(int maximumCapacity)
            : base(maximumCapacity) { }

        public CircularBufferQueue(IEnumerable<TValue> collection)
            : base(collection) { }

        public void Queue(TValue item)
        {
            AddTail(item);
        }

        public TValue Dequeue()
        {
            return RemoveHead();
        }

        public TValue Peek()
        {
            return GetHead();
        }
    }
}
