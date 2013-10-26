using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.Container.Test
{
    public class EpubContainerTestWrapper : TestPrepareWrapper
    {
        private string epubPath;

        public EpubContainerTestWrapper(string epubPath)
            : base(new EpubContainerTest(epubPath))
        {
            this.epubPath = epubPath;
        }

        protected override void onTearUp()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(epubPath))
            {
                // Copy from res to iso
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(epubPath))
                {
                    res.CopyTo(iso);
                }
            }
        }

        protected override void onTearDown()
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(epubPath))
            {
                // Delete iso
                iso.Delete();
            }
        }
    }
}
