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
        public static readonly string Delimiter = "/";
        protected static readonly char[] DelimitarChars = Delimiter.ToCharArray();

        protected readonly string filename;

        /// <summary>
        /// Create a new Storage object using the specified filename
        /// </summary>
        /// <param name="filename"></param>
        protected AlbiteStorage(string filename)
        {
            this.filename = filename;
        }

        public string Filename
        {
            get
            {
                return filename;
            }
        }

        /// <summary>
        /// Creates a new AlbiteStorage, relative to this one
        /// </summary>
        /// <param name="filename">the filename of the new storage</param>
        /// <returns></returns>
        public abstract AlbiteStorage OpenRelative(string filename);

        protected abstract Stream getStream(FileMode fileMode);
        protected abstract void CreatePathForFile();
        public abstract void Dispose();

        /// <summary>
        /// Gets the reading stream for this file
        /// </summary>
        /// <returns>A stream ready for reading</returns>
        public Stream ReadAsStream()
        {
            return getStream(FileMode.Open);
        }

        /// <summary>
        /// Gets the file contents into a byte array
        /// </summary>
        /// <returns>A byte array with the file contents</returns>
        public byte[] ReadAsBytes()
        {
            byte[] data;

            using (Stream stream = ReadAsStream())
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    data = reader.ReadBytes((int)stream.Length);
                }
            }

            return data;
        }

        /// <summary>
        /// Reads the file contents into a string
        /// </summary>
        /// <param name="encoding">The Encoding the string has been encoded with</param>
        /// <returns>A string representing the file content</returns>
        public string ReadAsString(Encoding encoding)
        {
            string s;

            using (Stream stream = ReadAsStream())
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    s = reader.ReadToEnd();
                }
            }

            return s;
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
            CreatePathForFile();
            using (Stream stream = getStream(FileMode.Create))
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
            CreatePathForFile();
            using (Stream stream = getStream(FileMode.Create))
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
            CreatePathForFile();
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
            using (Stream inputStream = ReadAsStream())
            {
                other.Write(inputStream);
            }
        }
    }
}
