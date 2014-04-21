using Albite.Reader.Core.Json;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Albite.Reader.Storage
{
    [DataContract]
    public class StorageItem
    {
        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public bool IsFolder { get; private set; }

        public ImageSource FileIcon { get; private set; }

        public StorageItem(string id, string name, bool isFolder, ImageSource fileIcon)
        {
            Id = id;
            Name = name;
            IsFolder = isFolder;
            FileIcon = fileIcon;
        }
    }
}
