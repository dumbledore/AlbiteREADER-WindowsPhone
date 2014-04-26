using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Albite.Reader.Core.Net;
using Albite.Reader.Core.Xml.Atom;
using System.Windows.Media;

namespace Albite.Reader.Storage.Services
{
    public abstract class OpdsService : StorageService
    {
        private StorageFolder root_;
        public IStorageFolder Root { get { return root_; } }

        protected abstract IEnumerable<string> SupportedMimetypes { get; }

        public OpdsService(string rootFolderId)
        {
            root_ = new StorageFolder(rootFolderId, "root");
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
            // if it's a feed, get the link title directly
            if (entry is IAtomFeed)
            {
                return link.Title;
            }

            // It's an entry, so use the entrie's title
            string title = entry.Title;

            // And add the author (if any)
            if (entry.Author != null)
            {
                title += " by " + entry.Author;
            }

            return title;
        }

        private static void processEntry(
            IAtomEntry entry, IList<StorageItem> items,
            IEnumerable<string> supportedMimetypes, ImageSource fileIcon)
        {
            foreach (IAtomLink link in entry.Links)
            {
                IEnumerable<string> values = getTypeValues(link.Mimetype);

                if (values.Contains(AtomType) && values.Contains(OpdsCatalogProfile))
                {
                    // A valid opds link

                    if (link.Rel == "self")
                    {
                        // Skip self links
                        continue;
                    }

                    if (!isUriSupported(link.Uri))
                    {
                        // Skip unsupported Uris
                        continue;
                    }

                    // A valid link, which can be treated as a virtual folder
                    items.Add(new StorageFolder(link.Uri.ToString(), getLinkTitle(entry, link)));
                }
                else if (link.Rel == AcquisitionRel && supportedMimetypes.Contains(link.Mimetype))
                {
                    items.Add(new StorageFile(link.Uri.ToString(), getLinkTitle(entry, link), fileIcon));
                }
            }
        }

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

                // process the feed
                processEntry(feed, items, SupportedMimetypes, fileIcon);

                // process the entries
                foreach (IAtomEntry entry in feed.Entries)
                {
                    processEntry(entry, items, SupportedMimetypes, fileIcon);
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

            return client.DownloadAsync(uri, ct);
        }
    }
}
