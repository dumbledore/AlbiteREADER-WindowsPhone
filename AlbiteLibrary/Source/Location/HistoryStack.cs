using SvetlinAnkov.Albite.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class HistoryStack : IContextAttachable<BookPresenter>
    {
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

        private readonly Stack<BookLocation> history;
        private int bookId;

        public HistoryStack()
        {
            history = new Stack<BookLocation>();
        }

        internal HistoryStack(SerializedHistoryStack stack)
        {
            history = new Stack<BookLocation>(stack.Data);
            bookId = stack.BookId;
        }

        public void Push(BookLocation location)
        {
            throwIfNotAttached();
            history.Push(location);
        }

        public BookLocation Pop()
        {
            throwIfNotAttached();
            return history.Pop();
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
