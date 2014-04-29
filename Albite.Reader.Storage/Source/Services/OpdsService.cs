using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Albite.Reader.Core.Net;
using Albite.Reader.Core.Xml.Atom;
using System.Windows.Media;
using Albite.Reader.Core.App;

namespace Albite.Reader.Storage.Services
{
    public abstract class OpdsService : StorageService
    {
        private StorageFolder root_;
        public IStorageFolder Root { get { return root_; } }

        protected abstract IEnumerable<string> SupportedMimetypes { get; }

        public OpdsService(string rootFolderId)
        {
            root_ = new StorageFolder(rootFolderId, Name);
        }

        private static CachedResourceImage cachedImage
            = new CachedLocalResourceImage("/Resources/Images/opds.png");

        private static CachedResourceImage cachedImageDark
            = new CachedLocalResourceImage("/Resources/Images/opds-dark.png");

        public override ImageSource Icon
        {
            get
            {
                return ThemeInfo.ThemeIsDark ? cachedImageDark.Value : cachedImage.Value;
            }
        }

        private static readonly string AtomType = "application/atom+xml";
        private static readonly string OpdsCatalogProfile = "profile=opds-catalog";
        private static readonly string AcquisitionRel = "http://opds-spec.org/acquisition";

        private static IEnumerable<string> getTypeValues(string s)
        {
            string[] values = s.Split(new char[] { ';' });

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
            }

            return values;
        }

        private static bool isUriSupported(Uri uri)
        {
            if (uri == null)
            {
                return false;
            }

            // File scheme is supported but would not make much sense in
            // the context of Opds, i.e. a local Opds Service.
            // Also ignore https as preventative measure - most of these
            // would require login which is not supported
            return uri.Scheme == "http" || uri.Scheme == "ftp";
        }

        private static string getLinkTitle(IAtomEntry entry, IAtomLink link)
        {
            string title = link.Title;

            if (title == null || title == string.Empty)
            {
                // Try entrie's title
                title = entry.Title;
            }

            if (title == null || title == string.Empty)
            {
                // No luck
                title = "Untitled";
            }

            return title;
        }

        private static bool isLinkSupported(IAtomLink link)
        {
            IEnumerable<string> values = getTypeValues(link.Mimetype);

            if (!(values.Contains(AtomType)))
            {
                // support only atom opds catolog links
                return false;
            }

            switch (link.Rel)
            {
                // Skip links that only clutter
                //case "related": // should we remove those?
                case "self":
                case "search":
                case "start":
                case "previous":
                case "alternate": // on FeedBooks it returns pages that
                                  // don't have acqusation links, so skip it
                    return false;
            }

            if (!isUriSupported(link.Uri))
            {
                // Skip unsupported Uris
                return false;
            }

            return true;
        }

        private static void processEntry(
            IAtomEntry entry, IList<StorageItem> items,
            IEnumerable<string> supportedMimetypes, ImageSource fileIcon)
        {
            foreach (IAtomLink link in entry.Links)
            {
                if (isLinkSupported(link))
                {
                    // A valid opds link, which can be treated as a virtual folder
                    items.Add(new StorageFolder(link.Uri.ToString(), getLinkTitle(entry, link)));
                }
                else if (supportedMimetypes.Contains(link.Mimetype))
                {
                    // It's not an OPDS link, is it's an acquisition link
                    items.Add(new StorageFile(link.Uri.ToString(), getLinkTitle(entry, link), fileIcon));
                }
            }
        }

        private static readonly string MorePrefix = "More of '";
        private static readonly string MoreSuffix = "'";

        public override async Task<ICollection<IStorageItem>> GetFolderContentsAsync(IStorageFolder folder, CancellationToken ct)
        {
            if (folder == null)
            {
                folder = Root;
            }

            // Create a webclient for the download
            AlbiteWebClient client = new AlbiteWebClient();

            // Create a uri from the folder id
            Uri uri = new Uri(folder.Id);

            using (Stream stream = await client.DownloadAsync(uri, ct))
            {
                IAtomFeed feed = new AtomReader(stream, uri).Feed;
                List<StorageItem> items = new List<StorageItem>();

                ImageSource fileIcon = null;
                if (GetFileIconDelegate != null)
                {
                    fileIcon = GetFileIconDelegate();
                }

                IEnumerable<IAtomEntry> entries = feed.Entries;

                if (entries.Count() > 0)
                {
                    // not an empty feed, so get links only from the entries
                    // problem is: the links in the feed are usually totally useless
                    // and without any context...
                    foreach (IAtomEntry entry in feed.Entries)
                    {
                        processEntry(entry, items, SupportedMimetypes, fileIcon);
                    }

                    // Add a "More" button only, if we actually got some results
                    // (or more precisely, some passed thorugh the filter)
                    if (items.Count() > 0)
                    {
                        // still, check for a "next" link, it's important that we get it
                        IAtomLink nextLink = feed.Links.FirstOrDefault(x => x.Rel == "next");
                        if (nextLink != null)
                        {
                            // It still has to be valid
                            if (isLinkSupported(nextLink))
                            {
                                // Add a "next" link. Do not use it's title,
                                // sometimes it may not have one!
                                string title = folder.Name;

                                if (!title.StartsWith(MorePrefix))
                                {
                                    title = MorePrefix + title + MoreSuffix;
                                }

                                items.Add(new StorageFolder(nextLink.Uri.ToString(), title));
                            }
                        }
                    }
                }
                else
                {
                    // empty feed, therefore it's the only entry
                    processEntry(feed, items, SupportedMimetypes, fileIcon);
                }

                return items.ToArray();
            }
        }

        public override Task<Stream> GetFileContentsAsync(IStorageFile file, CancellationToken ct, IProgress<double> progress)
        {
            // Create a webclient for the download
            AlbiteWebClient client = new AlbiteWebClient();

            // Create a uri from the file id
            Uri uri = new Uri(file.Id);

            return client.DownloadAsync(uri, ct, progress);
        }

        // IsSearchSupported is not overriden here, so it's up to the
        // concrete OPDS service to enable it or not.
        protected virtual string GetSearchUrl(string query)
        {
            throw new InvalidOperationException("Search is not supported");
        }

        public override IStorageFolder GetSearchFolder(string query)
        {
            // No need to escape the Uri string as looks like it's done in WebClient already
            return new StorageFolder(GetSearchUrl(query), "Search for " + query);
        }
    }
}
