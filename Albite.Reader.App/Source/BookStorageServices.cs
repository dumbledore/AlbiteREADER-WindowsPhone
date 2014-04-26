using Albite.Reader.Container;
using Albite.Reader.Core.App;
using Albite.Reader.Storage;
using Albite.Reader.Storage.Services;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Albite.Reader.App.Browse
{
    public static class BookStorageServices
    {
        private static readonly StorageService[] services_ =
        {
            new ExternalStorageService(),
            new OneDriveBrowsingService(),
            new FeedBooksService(),
        };

        static BookStorageServices()
        {
            foreach (StorageService service in services_)
            {
                service.IsFileAcceptedDelegate = isFileAccepted;
                service.GetFileIconDelegate = getFileIcon;
            }
        }

        public static ICollection<StorageService> Services
        {
            get
            {
                return Array.AsReadOnly<StorageService>(services_);
            }
        }

        public static StorageService GetService(string id)
        {
            foreach (StorageService service in services_)
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

        private class FeedBooksService : OpdsService
        {
            // Go directly to books only from the public domain
            private static readonly string Url = "http://www.feedbooks.com/publicdomain/catalog.atom";

            protected override IEnumerable<string> SupportedMimetypes
            {
                get { return BookContainer.SupportedMimetypes; }
            }

            public FeedBooksService() : base(Url) { }

            public override string Name
            {
                get { return "FeedBooks"; }
            }

            public override string Id
            {
                get { return "feedbooks"; }
            }
        }
    }
}
