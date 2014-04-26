using System.Windows.Media;

namespace Albite.Reader.Storage
{
    internal class StorageFile : StorageItem, IStorageFile
    {
        public ImageSource FileIcon { get; private set; }

        public StorageFile(string id, string name, ImageSource fileIcon)
            : base(id, name)
        {
            FileIcon = fileIcon;
        }
    }
}
