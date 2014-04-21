using System.Windows;
using Albite.Reader.Storage;
using Albite.Reader.Core.App;

namespace Albite.Reader.App.View.Controls
{
    public class FolderControl : IconContentControl
    {
        public static readonly DependencyProperty FolderItemProperty
            = DependencyProperty.Register("FolderItem", typeof(StorageItem), typeof(FolderControl),
            new PropertyMetadata(onFolderItemChanged));

        public StorageItem FolderItem
        {
            get { return (StorageItem)GetValue(FolderItemProperty); }
            set { SetValue(FolderItemProperty, value); }
        }

        private static CachedResourceImage cachedFolderImage
            = new CachedResourceImage("/Resources/Images/folder.png");

        private static CachedResourceImage cachedFolderImageDark
            = new CachedResourceImage("/Resources/Images/folder-dark.png");

        private static void onFolderItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FolderControl control = (FolderControl)d;

            StorageItem newValue = (StorageItem)e.NewValue;

            control.ContentTextBlock.Text = newValue.Name;

            control.Icon.Source = newValue.IsFolder
                ? (ThemeInfo.ThemeIsDark ? cachedFolderImageDark.Value : cachedFolderImage.Value)
                : newValue.FileIcon;
        }
    }
}
