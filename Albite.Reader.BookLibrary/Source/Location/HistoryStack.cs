using Albite.Reader.Core.Collections;
using Albite.Reader.Core.Serialization;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Albite.Reader.BookLibrary.Location
{
    public class HistoryStack : IContextAttachable<BookPresenter>
    {
        public static int MaximumCapacity
        {
            get { return 12; }
        }

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
            if (!history.IsEmpty)
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

        private readonly CircularBufferStack<BookLocation> history;

        private int bookId;

        public HistoryStack(BookPresenter bookPresenter)
        {
            // Set up the context
            context_ = bookPresenter;
            bookId = context_.Book.Id;

            // Create a new list to hold the locations
            history = new CircularBufferStack<BookLocation>(MaximumCapacity);
        }

        internal HistoryStack(SerializedHistoryStack stack)
        {
            history = new CircularBufferStack<BookLocation>(stack.Data, MaximumCapacity);
            bookId = stack.BookId;
        }

        public BookLocation GetCurrentLocation()
        {
            throwIfNotAttached();

            BookLocation location;

            if (history.IsEmpty)
            {
                location = context_.Spine[0].CreateLocation(ChapterLocation.Default);
            }
            else
            {
                location = history.Peek();
            }

            return location;
        }

        public void SetCurrentLocation(BookLocation location)
        {
            throwIfNotAttached();

            if (history.IsEmpty)
            {
                history.Push(location);
            }
            else
            {
                history.Update(location);
            }
        }

        // Needs to have more than one item.
        // First item is always the last (current) location
        public bool HasHistory
        {
            get
            {
                throwIfNotAttached();
                return history.Count > 1;
            }
        }

        public void AddNewLocation(BookLocation location)
        {
            throwIfNotAttached();
            history.Push(location);
        }

        public BookLocation GetPreviousLocation()
        {
            if (!HasHistory)
            {
                throw new InvalidOperationException("No history");
            }

            // remove current location
            history.Pop();

            // return previous location
            return history.Peek();
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
                ISerializer<object> serializer = LocationSerializer.CreateSerializer();
                SerializedHistoryStack stack = (SerializedHistoryStack)serializer.Decode(encodedData);
                return new HistoryStack(stack);
            }

            public override string ToString()
            {
                ISerializer<object> serializer = LocationSerializer.CreateSerializer();
                return serializer.Encode(this);
            }
        }
    }
}
