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
        private Stack<StorageItem> history;

        public FolderHistoryStack()
        {
            history = new Stack<StorageItem>();
        }

        private FolderHistoryStack(SerializedHistoryStack stack)
        {
            history = new Stack<StorageItem>(stack.Data);
        }

        public bool CanGoBack { get { return history.Count > 0; } }

        public StorageItem CurrentFolder
        {
            get { return history.Count > 0 ? history.Peek() : null; }
        }

        public StorageItem GoForward(StorageItem folder)
        {
            if (!folder.IsFolder)
            {
                throw new InvalidOperationException("provided item is not a folder");
            }

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

        public StorageItem GoBack()
        {
            if (history.Count == 0)
            {
                throw new InvalidOperationException("Already in root, i.e. no history");
            }

            // Remove current
            history.Pop();

            // Previous folder
            StorageItem folder = CurrentFolder;

            // Callback
            if (FolderChangedDelegate != null)
            {
                FolderChangedDelegate(folder);
            }

            // Return parent folder
            return folder;
        }

        public delegate void FolderChanged(StorageItem folder);

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
            public StorageItem[] Data { get; private set; }

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
                    typeof(StorageItem),
            };

            private static JsonSerializer<object> getSerializer()
            {
                return new JsonSerializer<object>(ExpectedTypes);
            }
        }
    }
}
