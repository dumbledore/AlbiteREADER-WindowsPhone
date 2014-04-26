using System.Runtime.Serialization;

namespace Albite.Reader.Storage
{
    [DataContract]
    internal class StorageItem : IStorageItem
    {
        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        public StorageItem(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
