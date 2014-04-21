using Albite.Reader.Core.App;
using Albite.Reader.Core.Diagnostics;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Albite.Reader.Storage.Services
{
    public class OneDriveBrowsingService : StorageService
    {
        private static readonly string ClientId = "ba7a5dbf-a049-48a1-b68c-f615ff680d6f";

        private static readonly string Tag = "OneDrive";

        private static readonly string[] LoginScopes =
            new string[]
        {
            "wl.signin",
            "wl.skydrive",
            "wl.offline_access",
        };

        public override string Name { get { return "OneDrive"; } }

        public override string Id { get { return "onedrive"; } }

        private static CachedResourceImage cachedImage
            = new CachedLocalResourceImage("/Resources/Images/onedrive.png");

        private static CachedResourceImage cachedImageDark
            = new CachedLocalResourceImage("/Resources/Images/onedrive-dark.png");

        private static StorageItem RootFolder = new StorageItem("me/skydrive", "root", true, null);

        public override ImageSource Icon
        {
            get
            {
                return ThemeInfo.ThemeIsDark ? cachedImageDark.Value : cachedImage.Value;
            }
        }

        public override bool LoginRequired { get { return true; } }

        private LiveAuthClient client = null;

        public override async Task LogIn()
        {
            if (LoggedIn)
            {
                // Already logged in
                return;
            }

            try
            {
                LiveAuthClient client = new LiveAuthClient(ClientId);

                LiveLoginResult result = await client.InitializeAsync();

                if (result == null || result.Status != LiveConnectSessionStatus.Connected)
                {
                    result = await client.LoginAsync(LoginScopes);
                }

                if (result.Status != LiveConnectSessionStatus.Connected)
                {
                    throw new InvalidOperationException("Couldn't connect");
                }

                // Connected
                this.client = client;
            }
            catch (LiveConnectException e)
            {
                throw new StorageException(e.Message, e);
            }
        }

        public override void LogOut()
        {
            if (client != null)
            {
                try
                {
                    client.Logout();
                    client = null;
                }
                catch (LiveConnectException e)
                {
                    throw new StorageException(e.Message, e);
                }
            }
        }

        public override bool LoggedIn
        {
            get
            {
                if (client == null || client.Session == null)
                {
                    return false;
                }

                TimeSpan diff = client.Session.Expires - DateTimeOffset.Now;

                // If there's one minute or less before session expiration,
                // report false, so that the session would be refreshed in time
                return diff.TotalMinutes > 1;
            }
        }

        public override async Task<ICollection<StorageItem>> GetFolderContentsAsync(
            StorageItem folder, CancellationToken ct)
        {
            if (folder == null)
            {
                folder = RootFolder;
            }

            if (!folder.IsFolder)
            {
                throw new InvalidOperationException("provided item is not a folder");
            }

            try
            {
                if (!LoggedIn)
                {
                    await LogIn();
                }

                Log.D(Tag, "Getting folder contents of " + folder.Name);

                // Connect to OneDrive
                LiveConnectClient connectClient = new LiveConnectClient(client.Session);

                // Wait for objects properties (folder's contents in our case)
                LiveOperationResult operationResult = await connectClient.GetAsync(folder.Id + "/files", ct);

                dynamic result = operationResult.Result;
                dynamic data = result.data;

                List<StorageItem> items = new List<StorageItem>(data.Count);

                ImageSource fileIcon = null;
                if (GetFileIconDelegate != null)
                {
                    fileIcon = GetFileIconDelegate();
                }

                foreach (dynamic d in data)
                {
                    string id = (string)d.id;
                    string name = (string)d.name;
                    string type = (string)d.type;

                    if (type == "folder")
                    {
                        items.Add(new StorageItem(id, name, true, null));
                    }
                    else if (type == "file")
                    {
                        if (IsFileAcceptedDelegate == null
                            || IsFileAcceptedDelegate(name))
                        {
                            items.Add(new StorageItem(id, name, false, fileIcon));
                        }
                    }

                    // skip non-folder/file types, e.g. albums
                }

                return items.ToArray();
            }
            catch (LiveConnectException e)
            {
                throw new StorageException(e.Message, e);
            }
        }

        public override async Task<Stream> GetFileContentsAsync(
            StorageItem file, CancellationToken ct, IProgress<double> progress)
        {
            if (file.IsFolder)
            {
                throw new InvalidOperationException("provided item is not a file");
            }

            try
            {
                if (!LoggedIn)
                {
                    await LogIn();
                }

                Log.D(Tag, "Getting file contents of " + file.Name);

                LiveConnectClient connectClient = new LiveConnectClient(client.Session);

                ProgressProxy progressProxy = progress == null ? null : new ProgressProxy(progress);

                LiveDownloadOperationResult operationResult
                    = await connectClient.DownloadAsync(file.Id + "/content", ct, progressProxy);

                return operationResult.Stream;
            }
            catch (LiveConnectException e)
            {
                throw new StorageException(e.Message, e);
            }
        }

        private class ProgressProxy : IProgress<LiveOperationProgress>
        {
            private IProgress<double> progress;

            public ProgressProxy(IProgress<double> progress)
            {
                this.progress = progress;
            }

            public void Report(LiveOperationProgress value)
            {
                progress.Report(value.ProgressPercentage);
            }
        }
    }
}
