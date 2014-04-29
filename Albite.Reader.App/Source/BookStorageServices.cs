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
            // Physical storage
            new ExternalStorageService(),

            // Cloud services
            new OneDriveBrowsingService(),

            // OPDS services. Check http://wiki.mobileread.com/wiki/OPDS
            new Service(
                "http://www.feedbooks.com/publicdomain/catalog.atom",
                "http://www.feedbooks.com/search.atom?query=",
                "FeedBooks", "feedbooks"),
            new Service(
                "http://m.gutenberg.org/ebooks/?format=opds",
                "http://m.gutenberg.org/ebooks/search.opds/?query=",
                "Project Gutenberg", "project-gutenberg"),
            new Service(
                "http://manybooks.net/opds/index.php",
                null, // skip the search as it only points to the web page, so it's useless
                "ManyBooks.net", "manybooks"),
            new Service(
                "http://www.baenebooks.com/stanza.aspx?feed=free",
                null, // Skip the search as it lists all its books anyway
                "Baen Ebooks", "baen-ebooks"),
            new Service(
                "http://pragprog.com/magazines.opds",
                null,
                "The Pragmatic Bookshelf", "pragmatic-bookshelf"),
            new Service(
                "http://www.ebooksgratuits.com/opds/",
                "http://www.ebooksgratuits.com/opds/feed.php?mode=search&query=",
                "Ebooks gratuits", "ebooks-gratuits"),
            new Service(
                "http://lib.rus.ec/opds",
                "http://lib.rus.ec/searchopds?ask=",
                "Либрусек", "librusek"),
            new Service(
                "http://coollib.com/opds",
                "http://coollib.com/opds/search?searchTerm=",
                "КулЛиб", "coollib-net"),
            new Service(
                "http://chitanka.info/catalog.opds",
                null,
                "Читанка", "chitanka"),
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

        private class Service : OpdsService
        {
            private string searchUrl_;
            private string name_;
            private string id_;

            public override string Name { get { return name_; } }
            public override string Id { get { return id_; } }

            public Service(string url, string searchUrl, string name, string id)
                : base(url)
            {
                searchUrl_ = searchUrl;
                name_ = name;
                id_ = id;
            }

            protected override IEnumerable<string> SupportedMimetypes
            {
                get { return BookContainer.SupportedMimetypes; }
            }

            public override bool IsSearchSupported
            {
                get { return searchUrl_ != null; }
            }

            protected override string GetSearchUrl(string query)
            {
                return searchUrl_ + query;
            }
        }
    }
}
