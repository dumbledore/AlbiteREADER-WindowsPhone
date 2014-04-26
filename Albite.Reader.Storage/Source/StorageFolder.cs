using System.Runtime.Serialization;

namespace Albite.Reader.Storage
{
    [DataContract]
    internal class StorageFolder : StorageItem, IStorageFolder
    {
        public StorageFolder(string id, string name)
            : base(id, name) { }
    }
}
