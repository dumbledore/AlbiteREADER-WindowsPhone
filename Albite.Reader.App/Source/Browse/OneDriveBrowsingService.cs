using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Albite.Reader.App.Browse
{
    public class OneDriveBrowsingService : IBrowsingService
    {
        private OneDriveBrowsingService() { }

        private static readonly OneDriveBrowsingService instance_ = new OneDriveBrowsingService();

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

        public void LogIn()
        {
            //TODO
        }

        public void LogOut()
        {
            // TODO
        }

        public bool LoggedIn
        {
            get
            {
                //TODO
                return false;
            }
        }

        public ICollection<IFolderItem> GetFolderContentsAsync(string path)
        {
            //TODO
            return null;
        }

        public Stream GetFileContentsAsync(string path)
        {
            //TODO
            return null;
        }
    }
}
