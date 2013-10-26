using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.Core.Test;
using SvetlinAnkov.Albite.Core.Utils;
using System.IO;

namespace SvetlinAnkov.Albite.Tests.Utils
{
    public class AlbiteIsolatedContainerTest : TestCase
    {
        string basePath;
        string[] entityNames;

        public AlbiteIsolatedContainerTest(string basePath, string[] entityNames)
        {
            this.basePath = basePath;
            this.entityNames = entityNames;
        }

        protected override void TestImplementation()
        {
            using (AlbiteIsolatedContainer container = new AlbiteIsolatedContainer(basePath))
            {
                foreach (string entityName in entityNames)
                {
                    byte[] buffer = new byte[AlbiteIsolatedStorage.BufferSize];

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
