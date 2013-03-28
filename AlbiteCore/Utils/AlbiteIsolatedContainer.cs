using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class AlbiteIsolatedContainer : IAlbiteContainer
    {
        private string basePath;

        private static Uri rootUri = new Uri("file:///");
        private static Uri baseUri = new Uri(rootUri, "/");

        public AlbiteIsolatedContainer(string basePath)
        {
            this.basePath = basePath;
        }

        public Stream Stream(string entityName)
        {
            // Remove any '..' in the name
            Uri uri = new Uri(baseUri, entityName);
            Uri uriValidated = rootUri.MakeRelativeUri(uri);

            // Get the validated entity name
            string entityNameValidated = Uri.UnescapeDataString(uriValidated.ToString());

            // Now make the full path
            string filename = Path.Combine(basePath, entityNameValidated);

            // This will also dispose of the iso instance. After that the stream
            // would still be valid as can be seen from AlbiteIsolatedStorageTest
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
            {
                return iso.GetStream(FileAccess.Read);
            }
        }

        // Nothing to dispose of
        public void Dispose() { }
    }
}
