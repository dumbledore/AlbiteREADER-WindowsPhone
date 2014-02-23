using Albite.Reader.Core.Serialization;
using Albite.Reader.Core.Test;

namespace Albite.Reader.Core.Test
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

            // Clear just in case
            store.Clear();

            // Assert it's clean
            Assert(store.Count == 0);

            // Try adding something to the store
            store["Alice"] = "Wonderland";

            // Try checking if it's there
            Assert(store.ContainsKey("Alice"));

            // Try retrieving the data
            Assert("Wonderland" == store["Alice"]);

            // Try updating the data
            store["Alice"] = "Looking-glass";

            // And assert the update went through
            Assert("Looking-glass" == store["Alice"]);

            // Try getting something that we never put
            string flamingo;
            Assert(!store.TryGetValue("Flamingo", out flamingo));
            Assert(flamingo == null);

            // Add one more pair
            store.Add("Rabbit", "White");

            // Now assert that TryGetValue works for existing stuff
            string white;
            Assert(store.TryGetValue("Rabbit", out white));

            // And it's indeed what it should be
            Assert("White" == white);

            // Remove it
            Assert(store.Remove("Rabbit"));

            // shouldn't be there anymore
            Assert(!store.ContainsKey("Rabbit"));

            // Clear again
            store.Clear();

            // And it's indeed empty
            Assert(store.Count == 0);
        }
    }
}
