using Albite.Reader.Core.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Albite.Reader.Storage
{
    [DataContract]
    public class FolderHistoryStack
    {
        [DataMember]
        private Stack<StorageFolder> history;

        public FolderHistoryStack()
        {
            history = new Stack<StorageFolder>();
        }

        private FolderHistoryStack(SerializedHistoryStack stack)
        {
            history = new Stack<StorageFolder>(stack.Data);
        }

        public bool CanGoBack { get { return history.Count > 0; } }

        public StorageFolder CurrentFolder
        {
            get { return history.Count > 0 ? history.Peek() : null; }
        }

        public StorageFolder GoForward(StorageFolder folder)
        {
            // Add to history
            history.Push(folder);

            // Callback
            if (FolderChangedDelegate != null)
            {
                FolderChangedDelegate(folder);
            }

            // Return new folder
            return folder;
        }

        public StorageFolder GoBack()
        {
            if (history.Count == 0)
            {
                throw new InvalidOperationException("Already in root, i.e. no history");
            }

            // Remove current
            history.Pop();

            // Previous folder
            StorageFolder folder = CurrentFolder;

            // Callback
            if (FolderChangedDelegate != null)
            {
                FolderChangedDelegate(folder);
            }

            // Return parent folder
            return folder;
        }

        public delegate void FolderChanged(StorageFolder folder);

        public FolderChanged FolderChangedDelegate { get; set; }

        public static FolderHistoryStack FromString(string encodedData)
        {
            return SerializedHistoryStack.FromString(encodedData);
        }

        public override string ToString()
        {
            SerializedHistoryStack stack = new SerializedHistoryStack(this);
            return stack.ToString();
        }

        [DataContract]
        private class SerializedHistoryStack
        {
            public SerializedHistoryStack(FolderHistoryStack stack)
            {
                // Stack -> Array
                Data = stack.history.ToArray();

                // Do not forget to reverse the Array as it represented a Stack
                // And the Stack<> constructor takes it in reversed order
                Array.Reverse(Data);
            }

            [DataMember]
            public StorageFolder[] Data { get; private set; }

            public static FolderHistoryStack FromString(string encodedData)
            {
                SerializedHistoryStack stack =
                    (SerializedHistoryStack)getSerializer().Decode(encodedData);
                return new FolderHistoryStack(stack);
            }

            public override string ToString()
            {
                return getSerializer().Encode(this);
            }

            private static readonly Type[] ExpectedTypes =
            {
                    typeof(SerializedHistoryStack),
                    typeof(StorageFolder),
            };

            private static JsonSerializer<object> getSerializer()
            {
                return new JsonSerializer<object>(ExpectedTypes);
            }
        }
    }
}
