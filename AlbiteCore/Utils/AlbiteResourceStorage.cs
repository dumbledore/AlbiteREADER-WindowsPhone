using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;
using System.IO;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class AlbiteResourceStorage : AlbiteStorage
    {
        private static readonly string ResourcesLocation = "Resources";

        public AlbiteResourceStorage(string filename) : base(filename) { }

        public override Stream GetStream(FileAccess access, FileMode mode, FileShare share)
        {
            StreamResourceInfo sr = Application.GetResourceStream(
                new Uri(Path.Combine(ResourcesLocation, FileName), UriKind.Relative));

            if (sr == null)
            {
                throw new FileNotFoundException(FileName);
            }

            return sr.Stream;
        }

        public override void Dispose() { }
    }
}
