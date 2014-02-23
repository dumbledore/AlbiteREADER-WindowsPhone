using Albite.Reader.Core.IO;
using Albite.Reader.Core.Test;
using System.IO;

namespace Albite.Reader.Core.Test
{
    public class IsolatedContainerTest : TestCase
    {
        string basePath;
        string[] entityNames;

        public IsolatedContainerTest(string basePath, string[] entityNames)
        {
            this.basePath = basePath;
            this.entityNames = entityNames;
        }

        protected override void TestImplementation()
        {
            using (IsolatedContainer container = new IsolatedContainer(basePath))
            {
                foreach (string entityName in entityNames)
                {
                    byte[] buffer = new byte[IsolatedStorage.BufferSize];

                    using (Stream stream = container.Stream(entityName))
                    {
                        int readBytes;
                        int readBytesTotal = 0;

                        while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            readBytesTotal += readBytes;
                        }

                        Log("Read {0} bytes for {1} : {2}", readBytesTotal, basePath, entityName);
                    }
                }
            }
        }
    }
}
