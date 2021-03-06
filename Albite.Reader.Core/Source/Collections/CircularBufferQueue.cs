﻿using System.Collections.Generic;

namespace Albite.Reader.Core.Collections
{
    public class CircularBufferQueue<TValue> : CircularBuffer<TValue>
    {
        public CircularBufferQueue(int maximumCapacity)
            : base(maximumCapacity) { }

        public CircularBufferQueue(IEnumerable<TValue> collection, int maximumCapacity)
            : base(collection, maximumCapacity) { }

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
