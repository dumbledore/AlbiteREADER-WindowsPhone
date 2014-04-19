using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Albite.Reader.App.Browse
{
    public class OneDriveBrowsingService : IBrowsingService
    {
        private OneDriveBrowsingService() { }

        private static readonly OneDriveBrowsingService instance_ = new OneDriveBrowsingService();

        private static readonly string ClientId = "ba7a5dbf-a049-48a1-b68c-f615ff680d6f";

        private static readonly string[] LoginScopes =
            new string[]
        {
            "wl.signin",
            "wl.basic",
            "wl.skydrive",
            "wl.offline_access",
        };

        public static IBrowsingService Instance
        {
            get { return instance_; }
        }

        public string Name { get { return "OneDrive"; } }

        public string Id { get { return "onedrive"; } }

        public ImageSource Icon
        {
            get
            {
                Uri uri = new Uri("/Resources/Images/onedrive.png", UriKind.Relative);
                return new BitmapImage(uri);
            }
        }

        public bool LoginRequired { get { return true; } }

        private LiveAuthClient client = null;

        public async Task LogIn()
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

        public void LogOut()
        {
            if (client == null)
            {
                return;
            }

            client.Logout();
            client = null;
        }

        public bool LoggedIn
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

        public async Task<ICollection<IFolderItem>> GetFolderContentsAsync(string path)
        {
            //TODO
            return await Task<ICollection<IFolderItem>>.Run(() =>
            {
                return new IFolderItem[0];
            });
    }

        public async Task<Stream> GetFileContentsAsync(string path)
        {
            // TODO
            return await Task<Stream>.Run(() =>
            {
                return Stream.Null;
            });
        }
    }
}
