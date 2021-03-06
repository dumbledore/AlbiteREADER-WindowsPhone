﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace Albite.Reader.Core.IO
{
    public class IsolatedContainer : IHashableContainer
    {
        private string basePath;

        private static RelativeUriResolver uriResolver = new RelativeUriResolver("/");

        public IsolatedContainer(string basePath)
        {
            this.basePath = basePath;
        }

        public Stream Stream(string entityName)
        {
            // Remove any '..' in the name
            string entityNameValidated = uriResolver.ResolveToString(entityName);

            // Now make the full path
            string filename = Path.Combine(basePath, entityNameValidated);

            return new StreamWrapper(filename);
        }

        public byte[] ComputeHash(HashAlgorithm hashAlgorithm)
        {
            throw new InvalidOperationException("Can't compute hash of isolated storage data");
        }

        // Nothing to dispose of
        public void Dispose() { }

        private class StreamWrapper : Stream
        {
            private readonly IsolatedStorage iso;
            private readonly Stream stream;

            public StreamWrapper(string filename)
            {
                this.iso = new IsolatedStorage(filename);
                this.stream = iso.GetStream(FileAccess.Read);
            }

            // This is called by Stream.Dispose()
            public override void Close()
            {
                // Pass this to Stream.Close() so that
                // it would call Dispose(true)
                base.Close();

                // Now pass it to the wrapped stream
                // just in case
                stream.Close();
            }

            // This will be called by Stream.Dispose()
            protected override void Dispose(bool disposing)
            {
                // Dispose the wrapped stream
                stream.Dispose();

                // Dispose the IsolatedStorageFile
                iso.Dispose();
            }

            #region Pass-through

            public override bool CanRead { get { return stream.CanRead; } }
            public override bool CanSeek { get { return stream.CanSeek; } }
            public override bool CanTimeout { get { return stream.CanTimeout; } }
            public override bool CanWrite { get { return stream.CanWrite; } }
            public override long Length { get { return stream.Length; } }

            public override long Position
            {
                get { return stream.Position; }
                set { stream.Position = value; }
            }

            public override int ReadTimeout
            {
                get { return stream.ReadTimeout; }
                set { stream.ReadTimeout = value; }
            }

            public override int WriteTimeout
            {
                get { return stream.WriteTimeout; }
                set { stream.WriteTimeout = value; }
            }

            public override IAsyncResult BeginRead(
                byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return stream.BeginRead(buffer, offset, count, callback, state);
            }

            public override IAsyncResult BeginWrite(
                byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return stream.BeginWrite(buffer, offset, count, callback, state);
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                return stream.EndRead(asyncResult);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                stream.EndWrite(asyncResult);
            }

            public override void Flush()
            {
                stream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return stream.Read(buffer, offset, count);
            }

            public override int ReadByte()
            {
                return stream.ReadByte();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                stream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                stream.Write(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                stream.WriteByte(value);
            }
            #endregion
        }
    }
}
