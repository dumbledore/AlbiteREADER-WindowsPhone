using System.Windows.Media;

namespace Albite.Reader.Storage
{
    public class StorageFile : IStorageItem
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public ImageSource FileIcon { get; private set; }

        internal StorageFile(string id, string name, ImageSource fileIcon)
        {
            Id = id;
            Name = name;
            FileIcon = fileIcon;
        }
    }
}
