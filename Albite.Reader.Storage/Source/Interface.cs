using System.Windows.Media;

namespace Albite.Reader.Storage
{
    public interface IStorageItem
    {
        string Id { get; }
        string Name { get; }
    }

    public interface IStorageFolder : IStorageItem { }

    public interface IStorageFile : IStorageItem
    {
        ImageSource FileIcon { get; }
    }
}
