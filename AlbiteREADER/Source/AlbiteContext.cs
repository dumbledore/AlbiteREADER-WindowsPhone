using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using SvetlinAnkov.Albite.READER.Model;
using SvetlinAnkov.Albite.Core.Utils;
using SvetlinAnkov.Albite.Library;

namespace SvetlinAnkov.Albite.READER
{
    public class AlbiteContext
    {
        public Library.Library Library { get; private set; }

        public AlbiteContext(string libraryPath)
        {
            Library = new Library.Library(libraryPath);
        }
    }
}
