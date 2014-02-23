using Albite.Reader.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Albite.Reader.Core.Test
{
    public class CircularBufferTest : TestCase
    {
        public int RandomIterations { get; private set; }

        public CircularBufferTest(int randomIterations)
        {
            RandomIterations = randomIterations;
        }

        protected override void TestImplementation()
        {
            manualTest();
            randomTest();
        }

        private void manualTest()
        {
            CircularBuffer<int> buffer = new CircularBuffer<int>(3);

            // check max capacity
            Assert(buffer.MaximumCapacity == 3);

            // check if empty
            Assert(buffer, new int[] { }, 0, true, false);

            // add to tail
            buffer.AddTail(1);

            // { 1 }
            Assert(buffer, new int[] { 1 }, 1, false, false);

            // add to tail
            buffer.AddTail(2);

            // { 1, 2 }
            Assert(buffer, new int[] { 1, 2 }, 2, false, false);

            // add to tail
            buffer.AddTail(3);

            // { 1, 2, 3 }
            Assert(buffer, new int[] { 1, 2, 3 }, 3, false, true);

            // add to tail
            buffer.AddTail(4);

            // { 2, 3, 4 }
            Assert(buffer, new int[] { 2, 3, 4 }, 3, false, true);

            // add to head
            buffer.AddHead(5);

            // { 5, 2, 3 }
            Assert(buffer, new int[] { 5, 2, 3 }, 3, false, true);

            // remove tail
            Assert(buffer.GetTail() == buffer.RemoveTail());

            // { 5, 2 }
            Assert(buffer, new int[] { 5, 2 }, 2, false, false);

            // add head
            buffer.AddHead(6);

            // { 6, 5, 2 }
            Assert(buffer, new int[] { 6, 5, 2 }, 3, false, true);

            // remove head
            Assert(buffer.GetHead() == buffer.RemoveHead());

            // { 5, 2 }
            Assert(buffer, new int[] { 5, 2 }, 2, false, false);

            // add head
            buffer.AddHead(7);

            // { 7, 5, 2 }
            Assert(buffer, new int[] { 7, 5, 2 }, 3, false, true);

            // add head
            buffer.AddHead(8);

            // { 8, 7, 5 }
            Assert(buffer, new int[] { 8, 7, 5 }, 3, false, true);

            // add head
            buffer.AddHead(9);

            // { 9, 8, 7 }
            Assert(buffer, new int[] { 9, 8, 7 }, 3, false, true);

            // remove tail
            Assert(buffer.GetTail() == buffer.RemoveTail());

            // { 9, 8 }
            Assert(buffer, new int[] { 9, 8 }, 2, false, false);

            // remove head
            Assert(buffer.GetHead() == buffer.RemoveHead());

            // { 8 }
            Assert(buffer, new int[] { 8 }, 1, false, false);

            // remove tail
            Assert(buffer.GetTail() == buffer.RemoveTail());

            // { }
            Assert(buffer, new int[] { }, 0, true, false);

            // try generating exceptions
            bool excepted;

            // GetHead() must throw exception on an empty list
            excepted = false;
            try
            {
                buffer.GetHead();
            }
            catch
            {
                excepted = true;
            }
            Assert(excepted);

            // RemoveHead() must throw exception on an empty list
            excepted = false;
            try
            {
                buffer.RemoveHead();
            }
            catch
            {
                excepted = true;
            }
            Assert(excepted);

            // GetTail() must throw exception on an empty list
            excepted = false;
            try
            {
                buffer.GetTail();
            }
            catch
            {
                excepted = true;
            }
            Assert(excepted);

            // RemoveTail() must throw exception on an empty list
            excepted = false;
            try
            {
                buffer.RemoveTail();
            }
            catch
            {
                excepted = true;
            }
            Assert(excepted);

            // add head
            buffer.AddHead(10);

            // { 10 }
            Assert(buffer, new int[] { 10 }, 1, false, false);

            // add head
            buffer.AddHead(11);

            // { 11, 10 }
            Assert(buffer, new int[] { 11, 10 }, 2, false, false);

            // Clear it
            buffer.Clear();

            // { }
            Assert(buffer, new int[] { }, 0, true, false);
        }

        private void randomTest()
        {
            int iterations = RandomIterations;
            int maxCapacity = 3;

            CircularBuffer<int> buffer = new CircularBuffer<int>(maxCapacity);

            List<int> list = new List<int>(maxCapacity);

            Random random = new Random();

            for (int i = 0; i < iterations; i++)
            {
                int number = random.Next(5);
                int item = i;
                int removedItem;

                // 0 -> remove head
                // 1 -> add head
                // 2 -> remove tail
                // 3 -> add tail
                // 4 -> clear

                if ((number == 0 || number == 2) && list.Count == 0)
                {
                    // in case of removing an item, but the lists are empty,
                    // default to adding
                    number++;
                }

                switch (number)
                {
                    case 0:
                        // asert heads are the same
                        removedItem = buffer.GetHead();
                        Assert(removedItem == list[0]);

                        // remove head from buffer
                        Assert(removedItem == buffer.RemoveHead());

                        // remove from list
                        list.RemoveAt(0);

                        // done
                        break;

                    case 1:
                        // add head to buffer
                        buffer.AddHead(item);

                        // remove tail if list is full
                        if (list.Count == maxCapacity)
                        {
                            list.RemoveAt(list.Count - 1);
                        }

                        // add head to list
                        list.Insert(0, item);

                        // done
                        break;

                    case 2:
                        // asert tails are the same
                        removedItem = buffer.GetTail();
                        Assert(removedItem == list[list.Count - 1]);

                        // remove tail from buffer
                        Assert(removedItem == buffer.RemoveTail());

                        // remove from list
                        list.RemoveAt(list.Count - 1);

                        // done
                        break;

                    case 3:
                        // add tail to buffer
                        buffer.AddTail(item);

                        // remove head if list is full
                        if (list.Count == maxCapacity)
                        {
                            list.RemoveAt(0);
                        }

                        // add tail to list
                        list.Add(item);

                        // done
                        break;

                    case 4:
                        // clear buffer
                        buffer.Clear();

                        // clear list
                        list.Clear();

                        // done
                        break;
                }

                // assert
                Assert(buffer, list, list.Count, list.Count == 0, list.Count == maxCapacity);
            }
        }

        private void Assert(IEnumerable<int> data1, IEnumerable<int> data2)
        {
            Assert(data2.SequenceEqual(data1));
        }

        public void Assert(CircularBuffer<int> buffer, IList<int> data, int count, bool isEmpty, bool isFull)
        {
            // items as expected?
            Assert(buffer, data);

            // count as expected?
            Assert(buffer.Count == count);

            // empty?
            Assert(buffer.IsEmpty == isEmpty);

            // full?
            Assert(buffer.IsFull == isFull);

            // for non-empty buffers
            if (data.Count > 0)
            {
                // head as expected?
                Assert(buffer.GetHead() == data[0]);

                // tail as expected?
                Assert(buffer.GetTail() == data[data.Count - 1]);
            }
        }
    }
}
