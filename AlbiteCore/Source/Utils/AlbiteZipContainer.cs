using System;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class AlbiteZipContainer : IAlbiteContainer
    {
        private readonly StreamResourceInfo info;
        private readonly Stream stream;

        public AlbiteZipContainer(Stream stream)
        {
            if (stream == null)
            {
                throw new NullReferenceException("Stream must not be null");
            }

            this.info = new StreamResourceInfo(stream, null);
            this.stream = stream;
        }

        public Stream Stream(string entityName)
        {
            // If it's an absolute path, make it relative.
            // It doesn't work with absolute paths / uris.
            char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            entityName = entityName.TrimStart(separators);

            Uri uri = new Uri(entityName, UriKind.Relative);
            StreamResourceInfo stream = Application.GetResourceStream(info, uri);

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
