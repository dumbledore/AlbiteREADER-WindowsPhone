using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.READER.Model;

namespace SvetlinAnkov.Albite.READER
{
    public class AlbiteContext : IDisposable
    {
        private readonly string libraryPath;

        private Library library;
        public Library Library
        {
            get
            {
                if (library == null)
                {
                    library = new Library(libraryPath);
                }
                return library;
            }
        }

        public AlbiteContext(string libraryPath)
        {
            this.libraryPath = libraryPath;
        }

        public void Dispose()
        {
            if (library != null)
            {
                library.Dispose();
                library = null;
            }
        }
    }
}
