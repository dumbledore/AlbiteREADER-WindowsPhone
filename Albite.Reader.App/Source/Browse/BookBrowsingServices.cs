using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Albite.Reader.App.Browse
{
    public static class BookBrowsingServices
    {
        private static readonly BrowsingService[] services_ =
        {
            new OneDriveBrowsingService(),
        };

        static BookBrowsingServices()
        {
            foreach (BrowsingService service in services_)
            {
                service.IsFileAcceptedDelegate = isFileAccepted;
                service.GetFileIconDelegate = getFileIcon;
            }
        }

        public static ICollection<BrowsingService> Services
        {
            get
            {
                return Array.AsReadOnly<BrowsingService>(services_);
            }
        }

        public static BrowsingService GetService(string id)
        {
            foreach (BrowsingService service in services_)
            {
                if (service.Id == id)
                {
                    return service;
                }
            }

            throw new InvalidOperationException("Unknown browsing service " + id);
        }

        private static bool isFileAccepted(string file)
        {
            return file.ToLowerInvariant().EndsWith(".epub");
        }

        private static CachedResourceImage cachedFileIcon
            = new CachedResourceImage("/Resources/Images/epub.png");

        private static CachedResourceImage cachedFileIconDark
            = new CachedResourceImage("/Resources/Images/epub-dark.png");

        private static ImageSource getFileIcon()
        {
            return ThemeInfo.ThemeIsDark ? cachedFileIconDark.Value : cachedFileIcon.Value;
        }
    }
}
