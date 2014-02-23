using Albite.Reader.Core.IO;
using System;
using System.IO;
using System.Text;

namespace Albite.Reader.Core.Diagnostics
{
    internal class IsolatedStorageLog : AbstractLog
    {
        public static string Tag { get { return "IsolatedStorageLog"; } }

        public static string DefaultLogLocation { get { return "application.log"; } }

        /// <summary>
        /// The log file is trimmed to this size.
        /// </summary>
        public static long TrimFileSize { get { return 32 * 1024; } }

        /// <summary>
        /// If the log file becomes larger than this, it gets trimmed.
        /// </summary>
        public static long MaxFileSize { get { return 256 * 1024; } }

        private IsolatedStorage store;
        private Stream stream;
        private BinaryWriter writer;
        private Encoding encoding = new UTF8Encoding();
        private Object myLock = new Object();

        public IsolatedStorageLog() : this(DefaultLogLocation) { }

        public IsolatedStorageLog(string location)
        {
            store = new IsolatedStorage(location);
            resetStream(FileMode.Append);
            logInternal(Level.Info, Tag, "=== Log Started ===", null);
        }

        private void resetStream(FileMode mode)
        {
            if (stream != null) { stream.Close(); }
            stream = store.GetStream(FileAccess.Write, mode, FileShare.ReadWrite);
            writer = new BinaryWriter(stream);
        }

        public override void Log(Level level, string tag, string message, Exception exception)
        {
            lock (myLock)
            {
                trimLogs();
                logInternal(level, tag, message, exception);
            }
        }
        
        private void trimLogs()
        {
            if (stream.Position < MaxFileSize)
            {
                return;
            }

            stream.Flush();
            long pos = stream.Position - TrimFileSize;
            if (pos <= 0)
            {
                // Something went wrong
                stream.SetLength(0);
                return;
            }

            resetStream(FileMode.Open);
            logInternal(Level.Info, Tag, "=== Trimmed ===", null);

            using (Stream inputStream = store.GetStream(FileAccess.Read, FileMode.Open, FileShare.ReadWrite))
            {
                inputStream.Position = pos;

                byte[] data = new byte[1024];

                int readTotal = 0;
                int read;
                while (readTotal < TrimFileSize)
                {
                    read = inputStream.Read(data, 0, data.Length);

                    // Nothing read, so nothing more to do.
                    if (read == 0) { break; }

                    stream.Write(data, 0, data.Length);
                    readTotal += read;
                }

                stream.SetLength(stream.Position);
            }
        }

        private void logInternal(Level level, string tag, string message, Exception exception)
        {
            writer.Write(encoding.GetBytes(LogAsString(level, tag, message, exception)));
            writer.Write('\n');
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            if (store != null)
            {
                store.Dispose();
                store = null;
            }
        }
    }
}
