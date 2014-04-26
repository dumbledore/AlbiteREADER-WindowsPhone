using Albite.Reader.Core.IO;
using Albite.Reader.Core.Xml.Atom;
using System.IO;

namespace Albite.Reader.Core.Test
{
    public class AtomReaderTest : TestCase
    {
        public string FilePath { get; private set; }

        public AtomReaderTest(string filePath)
        {
            FilePath = filePath;
        }

        protected override void TestImplementation()
        {
            using (ResourceStorage res = new ResourceStorage(FilePath))
            {
                using (Stream stream = res.GetStream(FileAccess.Read))
                {
                    AtomReader reader = new AtomReader(stream);
                    IAtomFeed feed = reader.Feed;
                    dumpEntry(feed);

                    if (feed.Entries != null)
                    {
                        foreach (IAtomEntry entry in feed.Entries)
                        {
                            dumpEntry(entry);
                        }
                    }
                }
            }
        }

        private void dumpEntry(IAtomEntry entry)
        {
            Log("{0} title={1} author={2}",
                entry is IAtomFeed ? "Feed" : "Entry", entry.Title, entry.Author);

            if (entry.Links != null)
            {
                foreach (IAtomLink link in entry.Links)
                {
                    dumpLink(link);
                }
            }
        }

        private void dumpLink(IAtomLink link)
        {
            Log("  Link title={0} rel={1} type={2} href={3}",
                link.Title, link.Rel, link.Mimetype, link.Uri);
        }
    }
}
