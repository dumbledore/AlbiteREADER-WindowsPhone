using Albite.Reader.Core.Diagnostics;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Albite.Reader.App.Browse
{
    public class OneDriveBrowsingService : BrowsingService
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
            = new CachedResourceImage("/Resources/Images/onedrive.png");

        private static CachedResourceImage cachedImageDark
            = new CachedResourceImage("/Resources/Images/onedrive-dark.png");

        private static FolderItem RootFolder = new FolderItem("me/skydrive", "root", true, null);

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

        public override void LogOut()
        {
            if (client == null)
            {
                return;
            }

            client.Logout();
            client = null;
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

        public override async Task<ICollection<FolderItem>> GetFolderContentsAsync(
            FolderItem folder, CancellationToken ct)
        {
            if (!LoggedIn)
            {
                await LogIn();
            }

            if (folder == null)
            {
                folder = RootFolder;
            }

            Log.D(Tag, "Getting folder contents of " + folder.Name);

            LiveConnectClient connectClient = new LiveConnectClient(client.Session);
            LiveOperationResult operationResult = await connectClient.GetAsync(folder.Id + "/files", ct);
            dynamic result = operationResult.Result;
            dynamic data = result.data;

            List<FolderItem> items = new List<FolderItem>(data.Count);

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
                    items.Add(new FolderItem(id, name, true, null));
                }
                else if (type == "file")
                {
                    if (IsFileAcceptedDelegate == null
                        || IsFileAcceptedDelegate(name))
                    {
                        items.Add(new FolderItem(id, name, false, fileIcon));
                    }
                }

                // skip non-folder/file types, e.g. albums
            }

            return items.ToArray();
        }

        public override async Task<Stream> GetFileContentsAsync(string path)
        {
            // TODO
            return await Task<Stream>.Run(() =>
            {
                return Stream.Null;
            });
        }
    }
}
