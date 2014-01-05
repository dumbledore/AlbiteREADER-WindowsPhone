using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SvetlinAnkov.Albite.Core.Collections
{
    public class CircularBuffer<TValue> : IEnumerable<TValue>, IEnumerable
    {
        public int MaximumCapacity
        {
            get { return data.Length; }
        }

        // array holding items
        protected TValue[] data;

        // offset of current item
        protected int offset;

        // total number of items
        protected int size;

        public CircularBuffer(int maximumCapacity)
        {
            if (maximumCapacity <= 0)
            {
                throw new ArgumentException("maximumCapacity must be positive");
            }

            data = new TValue[maximumCapacity];

            reset(false);
        }

        public CircularBuffer(IEnumerable<TValue> collection, int maximumCapacity)
        {
            if (collection == null)
            {
                throw new NullReferenceException("collection is null");
            }

            TValue[] data = collection.ToArray();

            if (data.Length == 0)
            {
                throw new ArgumentException("collection is empty");
            }

            if (data.Length > maximumCapacity)
            {
                throw new ArgumentException("collection has more data than provided maximumCapacity");
            }

            // reset
            reset(false);

            // get the current size <= maximumCapacity
            this.size = data.Length;

            // create the new data array
            this.data = new TValue[maximumCapacity];

            // copy to store array
            Array.Copy(data, this.data, size);
        }

        public void Clear()
        {
            reset(true);
        }

        private void reset(bool clear)
        {
            offset = 0;
            size = 0;

            if (clear)
            {
                // Clear the data, so that we don't leak resources
                Array.Clear(data, 0, data.Length);
            }
        }

        public int Count
        {
            get { return size; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public bool IsFull
        {
            get { return Count == MaximumCapacity; }
        }

        public void AddHead(TValue item)
        {
            offset--;

            if (offset < 0)
            {
                // This should mean that offset == -1
                // but we are handling it in a generic way
                offset = MaximumCapacity + offset;
            }

            data[offset] = item;

            if (!IsFull)
            {
                // increment the size only if not full
                size++;
            }
        }

        public TValue GetHead()
        {
            return getHead(false);
        }

        public TValue RemoveHead()
        {
            return getHead(true);
        }

        private TValue getHead(bool remove)
        {
            ThrowIfEmpty();

            // get head
            TValue item = data[offset];

            // remove?
            if (remove)
            {
                // don't leak
                data[offset] = default(TValue);

                // move offset to the right, not forgetting it might wrap
                offset = wrapIndex(offset + 1);

                // decrement total size
                size--;
            }

            return item;
        }

        public void AddTail(TValue item)
        {
            if (IsFull)
            {
                // move the head
                offset = wrapIndex(offset + 1);
            }
            else
            {
                size++;
            }

            int index = wrapIndex(offset + size - 1);

            data[index] = item;
        }

        public TValue GetTail()
        {
            return getTail(false);
        }

        public TValue RemoveTail()
        {
            return getTail(true);
        }

        private TValue getTail(bool remove)
        {
            ThrowIfEmpty();

            // get index
            int index = wrapIndex(offset + size - 1);

            // get head
            TValue item = data[index];

            // remove?
            if (remove)
            {
                // don't leak
                data[index] = default(TValue);

                // decrement total size
                size--;
            }

            return item;
        }

        protected void ThrowIfEmpty()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        private int wrapIndex(int index)
        {
            return index % MaximumCapacity;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<TValue>
        {
            TValue[] data;

            int offset;

            int size;

            int currentIndex;

            TValue currentItem;

            public Enumerator(CircularBuffer<TValue> buffer)
            {
                data = buffer.data;
                offset = buffer.offset;
                size = buffer.size;

                Reset();
            }

            public void Reset()
            {
                currentIndex = -1;
                currentItem = default(TValue);
            }

            public bool MoveNext()
            {
                bool hasNext = false;

                if (currentIndex + 1 < size)
                {
                    // increment to next index
                    currentIndex++;

                    // update current item
                    currentItem = data[(offset + currentIndex) % data.Length];

                    // there's an item
                    hasNext = true;
                }

                return hasNext;
            }

            void IDisposable.Dispose() { }

            public TValue Current
            {
                get { return currentItem; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
