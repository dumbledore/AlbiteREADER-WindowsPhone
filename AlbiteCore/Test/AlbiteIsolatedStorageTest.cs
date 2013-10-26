using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Core.Test;
using System.IO;

namespace SvetlinAnkov.Albite.Core.Test
{
    public class AlbiteIsolatedStorageTest : TestCase
    {
        private string filename;

        public AlbiteIsolatedStorageTest(string filename)
        {
            this.filename = filename;
        }

        protected override void TestImplementation()
        {
            // First copy the file from the resources to the iso
            using (AlbiteResourceStorage input = new AlbiteResourceStorage(filename))
            {
                using (AlbiteIsolatedStorage output = new AlbiteIsolatedStorage(filename))
                {
                    input.CopyTo(output);
                }
            }

            Stream stream;
            // Using will make sure that isf will have been disposed
            // by the time the stream is used which is the point of this
            // test case: to assert that the stream would still be available
            // even when the ISF is not.
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
            {
                // Get the stream.
                stream = iso.GetStream(FileAccess.Read);
            }

            // Allocate the buffer
            byte[] buffer = new byte[AlbiteIsolatedStorage.BufferSize];

            // Simply read and discard
            int bytesReadTotal = 0;
            int bytesRead = 0;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                bytesReadTotal += bytesRead;
            }

            Log("Totally read {0} bytes from {1}", bytesReadTotal, filename);

            // Close the stream explicitly!
            stream.Close();

            // Now delete from the storage
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
            {
                iso.Delete();
            }
        }
    }
}
