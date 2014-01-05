using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class HistoryStack : IContextAttachable<BookPresenter>
    {
        public static readonly int DefaultMaximumCapacity = 20;

        private readonly int maximumCapacity;

        private BookPresenter context_;

        public bool IsAttached
        {
            get { return context_ != null; }
        }

        public BookPresenter Context
        {
            get { return context_; }
        }

        public void Attach(BookPresenter context)
        {
            if (IsAttached)
            {
                // Can't reattach
                throw new InvalidOperationException("Already attached");
            }

            // Validate the context. While the stack is empty and not attached,
            // one can attach it to any context
            if (history.Count > 0)
            {
                if (bookId != context.Book.Id) // different book
                {
                    throw new InvalidOperationException("Invalid context");
                }

                // Attach all locations
                foreach (BookLocation location in history)
                {
                    location.Attach(context);
                }
            }

            // All set
            context_ = context;
        }

        private void throwIfNotAttached()
        {
            if (!IsAttached)
            {
                throw new InvalidOperationException("Not attached");
            }
        }

        private readonly List<BookLocation> history;
        private int bookId;


        public HistoryStack()
            : this(DefaultMaximumCapacity) { }

        public HistoryStack(int maximumCapacity)
        {
            if (maximumCapacity <= 0)
            {
                throw new ArgumentException("maximumCapacity must be positive");
            }

            this.maximumCapacity = maximumCapacity;

            // Create a new list to hold the locations
            history = new List<BookLocation>(maximumCapacity);
        }

        internal HistoryStack(SerializedHistoryStack stack)
        {
            history = new List<BookLocation>(stack.Data);
            maximumCapacity = stack.MaximumCapacity;
            bookId = stack.BookId;

            // trim the list in case it was serialized incorrectly
            trimItems(maximumCapacity);
        }

        public void Push(BookLocation location)
        {
            throwIfNotAttached();

            // trim to max - 1 as we are adding a new item
            trimItems(maximumCapacity - 1);

            // add to tail
            history.Add(location);
        }

        public BookLocation Pop()
        {
            throwIfNotAttached();

            if (history.Count == 0)
            {
                throw new InvalidOperationException("History stack is empty");
            }

            // tail index
            int index = history.Count - 1;

            // get tail
            BookLocation location = history[index];

            // remove tail from list
            history.RemoveAt(index);

            return location;
        }

        private void trimItems(int maxItems)
        {
            int itemsToRemove = history.Count - maxItems;

            if (itemsToRemove > 0)
            {
                history.RemoveRange(0, itemsToRemove);
            }
        }

        public bool IsEmpty
        {
            get
            {
                throwIfNotAttached();
                return history.Count == 0;
            }
        }

        public static HistoryStack FromString(string encodedData)
        {
            return SerializedHistoryStack.FromString(encodedData);
        }

        public override string ToString()
        {
            SerializedHistoryStack stack = new SerializedHistoryStack(this);
            return stack.ToString();
        }

        [DataContract(Name = "serializedHistoryStack")]
        internal class SerializedHistoryStack
        {
            public SerializedHistoryStack(HistoryStack stack)
            {
                Data = stack.history.ToArray();
                BookId = stack.Context.Book.Id;
            }

            [DataMember(Name = "maximumCapacity")]
            public int MaximumCapacity { get; private set; }

            [DataMember(Name = "data")]
            public BookLocation[] Data { get; private set; }

            [DataMember(Name = "bookId")]
            public int BookId { get; private set; }

            public static HistoryStack FromString(string encodedData)
            {
                if (encodedData == null || encodedData.Length == 0)
                {
                    return new HistoryStack();
                }

                LocationSerializer serializer = new LocationSerializer();
                SerializedHistoryStack stack = (SerializedHistoryStack)serializer.Decode(encodedData);
                return new HistoryStack(stack);
            }

            public override string ToString()
            {
                LocationSerializer serializer = new LocationSerializer();
                return serializer.Encode(this);
            }
        }
    }
}
