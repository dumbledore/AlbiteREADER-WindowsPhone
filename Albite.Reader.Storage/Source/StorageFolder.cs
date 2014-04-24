using System.Runtime.Serialization;

namespace Albite.Reader.Storage
{
    [DataContract]
    public class StorageFolder : IStorageItem
    {
        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        internal StorageFolder(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
