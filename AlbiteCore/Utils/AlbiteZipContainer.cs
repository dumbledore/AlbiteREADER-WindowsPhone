using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;
using System.Windows.Resources;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class AlbiteZipContainer : IAlbiteContainer
    {
        private readonly Stream stream;

        public AlbiteZipContainer(Stream stream)
        {
            if (stream == null)
            {
                throw new NullReferenceException("Stream must not be null");
            }

            this.stream = stream;
        }

        public Stream Stream(string entityName)
        {
            // If it's an absolute path, make it relative.
            // It doesn't work with absolute paths / uris.
            char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            entityName = entityName.TrimStart(separators);

            Uri uri = new Uri(entityName, UriKind.Relative);
            StreamResourceInfo info = new StreamResourceInfo(this.stream, null);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(info, uri);

            if (stream == null || stream.Stream == null)
            {
                throw new FileNotFoundException(entityName);
            }

            return stream.Stream;
        }

        public void UnzipStorage(string entityName, AlbiteStorage output)
        {
            using (Stream zipStream = Stream(entityName))
            {
                output.Write(zipStream);
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
