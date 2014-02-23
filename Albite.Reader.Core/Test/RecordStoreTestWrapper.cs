
using Albite.Reader.Core.IO;
namespace Albite.Reader.Core.Test
{
    public class RecordStoreTestWrapper : TestPrepareWrapper
    {
        public string FilePath { get; private set; }

        public RecordStoreTestWrapper(string filePath)
            : base(new RecordStoreTest(filePath))
        {
            FilePath = filePath;
        }

        protected override void onTearUp()
        {
            // Same as onTearDown
            onTearDown();
        }

        protected override void onTearDown()
        {
            // Delete iso file
            using (IsolatedStorage iso = new IsolatedStorage(FilePath))
            {
                //iso.Delete();
            }
        }
    }
}
