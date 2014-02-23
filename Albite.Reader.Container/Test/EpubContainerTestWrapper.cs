using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;

namespace Albite.Reader.Container.Test
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
            using (IsolatedStorage iso = new IsolatedStorage(epubPath))
            {
                // Copy from res to iso
                using (ResourceStorage res = new ResourceStorage(epubPath))
                {
                    res.CopyTo(iso);
                }
            }
        }

        protected override void onTearDown()
        {
            using (IsolatedStorage iso = new IsolatedStorage(epubPath))
            {
                // Delete iso
                iso.Delete();
            }
        }
    }
}
