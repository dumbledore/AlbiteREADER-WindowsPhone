using Albite.Reader.Core.App;
using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Albite.Reader.Storage.Services
{
    public class ExternalStorageService : SearchableStorageService
    {
        private ExternalStorageDevice externalStorage_ = null;
        private async Task<ExternalStorageDevice> getExternalStorage()
        {
            if (externalStorage_ == null)
            {
                // Cached the external storage
                externalStorage_ = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

                // If still null, no external storage found
                if (externalStorage_ == null)
                {
                    throw new StorageException("No external storage found");
                }
            }

            return externalStorage_;
        }

        public override string Name { get { return "SD card"; } }

        public override string Id { get { return "external"; } }

        private static CachedResourceImage cachedImage
            = new CachedLocalResourceImage("/Resources/Images/sdcard.png");

        private static CachedResourceImage cachedImageDark
            = new CachedLocalResourceImage("/Resources/Images/sdcard-dark.png");

        public override ImageSource Icon
        {
            get
            {
                return ThemeInfo.ThemeIsDark ? cachedImageDark.Value : cachedImage.Value;
            }
        }

        public override async Task<ICollection<IStorageItem>> GetFolderContentsAsync(IStorageFolder folder, CancellationToken ct)
        {
            string query = null;

            if (folder != null && GetSearchQuery(folder, ref query))
            {
                // A search is what is needed
                return await Search(query, ct);
            }

            // Get external storage
            ExternalStorageDevice externalStorage = await getExternalStorage();

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Get folder contents
            ExternalStorageFolder externalFolder = null;

            if (folder == null)
            {
                externalFolder = externalStorage.RootFolder;
            }
            else
            {
                externalFolder = await externalStorage.GetFolderAsync(folder.Id);
            }

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Get sub-folders
            IEnumerable<ExternalStorageFolder> subFolders = await externalFolder.GetFoldersAsync();

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Get files
            IEnumerable<ExternalStorageFile> files = await externalFolder.GetFilesAsync();

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Create list of items
            List<IStorageItem> items = new List<IStorageItem>();

            // Add folders to list
            foreach (ExternalStorageFolder subFolder in subFolders.OrderBy(x => x.Name))
            {
                items.Add(new StorageFolder(subFolder.Path, subFolder.Name));
            }

            ImageSource fileIcon = null;
            if (GetFileIconDelegate != null)
            {
                fileIcon = GetFileIconDelegate();
            }

            // Add files to list
            foreach (ExternalStorageFile file in files.OrderBy(x => x.Name))
            {
                if (IsFileAcceptedDelegate == null
                    || IsFileAcceptedDelegate(file.Name))
                {
                    items.Add(new StorageFile(file.Path, file.Name, fileIcon));
                    file.Dispose();
                }
            }

            return items.ToArray();
        }

        public override async Task<Stream> GetFileContentsAsync(IStorageFile file, CancellationToken ct, IProgress<double> progress)
        {
            // Get folder path
            string folder = Path.GetDirectoryName(file.Id);

            // Get file name
            string filename = Path.GetFileName(file.Id);

            // Get external storage
            ExternalStorageDevice externalStorage = await getExternalStorage();

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Get folder contents
            ExternalStorageFolder externalFolder = null;

            if (folder == string.Empty)
            {
                externalFolder = externalStorage.RootFolder;
            }
            else
            {
                externalFolder = await externalStorage.GetFolderAsync(folder);
            }

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Get files
            IEnumerable<ExternalStorageFile> files = await externalFolder.GetFilesAsync();

            // Have we been cancelled?
            ct.ThrowIfCancellationRequested();

            // Get the file from the list. This will rightly throw if the file's not in the list
            ExternalStorageFile externalFile = files.First(x => x.Path == file.Id);

            // Now get the stream to the contents!
            return await externalFile.OpenForReadAsync();
        }
    }
}
