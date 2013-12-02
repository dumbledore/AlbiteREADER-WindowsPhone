using SvetlinAnkov.Albite.Core.Serialization;
using SvetlinAnkov.Albite.Core.Test;

namespace SvetlinAnkov.Albite.Core.Test
{
    public class RecordStoreTest : TestCase
    {
        public string FilePath { get; private set; }

        public RecordStoreTest(string filePath)
        {
            FilePath = filePath;
        }

        protected override void TestImplementation()
        {
            RecordStore store = new RecordStore(FilePath);

            // Try adding something to the store
            store["Alice"] = "Wonderland";

            // Try retrieving the data
            Assert("Wonderland" == store["Alice"]);

            // Try updating the data
            store["Alice"] = "Looking-glass";

            // And assert the update went through
            Assert("Looking-glass" == store["Alice"]);

            // Try getting something that we never put
            Assert(!store.ContainsKey("Flamingo"));
        }
    }
}
