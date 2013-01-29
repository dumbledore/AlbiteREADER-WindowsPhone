using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Text;

namespace SvetlinAnkov.AlbiteREADER.Utils
{
    public abstract class AlbiteStorage : IDisposable
    {
        public static readonly int BufferSize = 1024;

        public string FileName { get; private set; }

        protected AlbiteStorage(string filename)
        {
            FileName = filename;
        }

        public abstract Stream GetStream(FileAccess access, FileMode mode, FileShare share);

        public Stream GetStream(FileAccess access, FileMode mode)
        {
            return GetStream(access, mode, FileShare.None);
        }

        public Stream GetStream(FileAccess access)
        {
            return GetStream(access, getModeForAccess(access));
        }

        private static FileMode getModeForAccess(FileAccess access)
        {
            switch (access)
            {
                case FileAccess.Read:
                    return FileMode.Open;

                case FileAccess.ReadWrite:
                    return FileMode.OpenOrCreate;

                case FileAccess.Write:
                    return FileMode.Create;
            }

            return FileMode.OpenOrCreate;
        }

        /// <summary>
        /// Gets the file contents into a byte array
        /// </summary>
        /// <returns>A byte array with the file contents</returns>
        public byte[] ReadAsBytes()
        {
            using (Stream stream = GetStream(FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)stream.Length);
                }
            }
        }

        /// <summary>
        /// Reads the file contents into a string
        /// </summary>
        /// <param name="encoding">The Encoding the string has been encoded with</param>
        /// <returns>A string representing the file content</returns>
        public string ReadAsString(Encoding encoding)
        {
            using (Stream stream = GetStream(FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Reads the file contents into a string using UTF8 encoding
        /// </summary>
        /// <returns>A string representing the file content</returns>
        public string ReadAsString()
        {
            return ReadAsString(new UTF8Encoding());
        }

        /// <summary>
        /// Saves the specified stream into the Isolated Storage
        /// </summary>
        /// <param name="inputStream">The stream to be saved</param>
        public void Write(Stream inputStream)
        {
            using (Stream stream = GetStream(FileAccess.Write))
            {
                byte[] buffer = new byte[BufferSize];
                for (int bytesRead = inputStream.Read(buffer, 0, buffer.Length); bytesRead > 0; bytesRead = inputStream.Read(buffer, 0, buffer.Length))
                {
                    stream.Write(buffer, 0, bytesRead);
                }
            }
        }

        /// <summary>
        /// Saves the specified data into the Isolated Storage
        /// </summary>
        /// <param name="data">The data to be saved</param>
        public void Write(byte[] data)
        {
            using (Stream stream = GetStream(FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(data);
                }
            }
        }

        /// <summary>
        /// Saves the specified string into the Isolated Storage in the specified Encoding
        /// </summary>
        /// <param name="s">The string to be saved</param>
        /// <param name="encoding">The encoding to be used</param>
        public void Write(string s, Encoding encoding)
        {
            Write(encoding.GetBytes(s));
        }

        /// <summary>
        /// Saves the specified string into the Isolated Storage in UTF-8
        /// </summary>
        /// <param name="s">The string to be saved</param>
        public void Write(string s)
        {
            Write(s, new UTF8Encoding());
        }

        /// <summary>
        /// Copies one storage file to another
        /// </summary>
        /// <param name="other"></param>
        public void CopyTo(AlbiteStorage other)
        {
            using (Stream inputStream = GetStream(FileAccess.Read))
            {
                other.Write(inputStream);
            }
        }

        public abstract void Dispose();
    }
}
