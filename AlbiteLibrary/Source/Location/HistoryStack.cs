using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SvetlinAnkov.Albite.BookLibrary.Location
{
    public class HistoryStack
    {
        private Stack<BookLocation> history;

        public HistoryStack()
        {
            history = new Stack<BookLocation>();
        }

        internal HistoryStack(SerializedHistoryStack stack)
        {
            history = new Stack<BookLocation>(stack.Data);
        }

        public void Push(BookLocation location)
        {
            history.Push(location);
        }

        public BookLocation Pop()
        {
            return history.Pop();
        }

        public bool IsEmpty
        {
            get { return history.Count == 0; }
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
            }

            [DataMember(Name = "data")]
            public BookLocation[] Data { get; private set; }

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
