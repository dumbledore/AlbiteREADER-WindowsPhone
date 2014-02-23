using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Resources;

namespace Albite.Reader.Core.IO
{
    public class ZipContainer : IHashableContainer
    {
        private readonly StreamResourceInfo info;
        private readonly Stream stream;

        public ZipContainer(Stream stream)
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

        public byte[] ComputeHash(HashAlgorithm hashAlgorithm)
        {
            // Get stream position
            long currentPosition = stream.Position;

            // Rewind stream to the beginning
            stream.Position = 0;

            // Compute hash value
            byte[] hash = hashAlgorithm.ComputeHash(stream);

            // Restore original position
            stream.Position = currentPosition;

            return hash;
        }

        public void UnzipStorage(string entityName, Storage output)
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
