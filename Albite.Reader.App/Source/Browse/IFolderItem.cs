using System.Windows.Media;

namespace Albite.Reader.App.Browse
{
    public interface IFolderItem
    {
        string Name { get; }
        bool IsFolder { get; }
        ImageSource FileIcon { get; }
    }
}
