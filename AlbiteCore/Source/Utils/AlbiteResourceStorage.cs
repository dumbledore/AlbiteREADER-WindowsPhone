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

        private string assemblyName = null;

        public AlbiteResourceStorage(string filename, string assemblyName)
            : base(filename)
        {
            this.assemblyName = assemblyName;
        }

        public AlbiteResourceStorage(string filename) : this(filename, null) { }

        public override Stream GetStream(FileAccess access, FileMode mode, FileShare share)
        {
            string path = Path.Combine(ResourcesLocation, FileName);

            if (assemblyName != null)
            {
                // The implemntation looks for forward slash '/' only,
                // i.e. ;component\ would not work and therefore
                // one can't use Path.Combine()
                path = assemblyName + ";component/" + path;
            }

            StreamResourceInfo sr = Application.GetResourceStream(
                new Uri(path, UriKind.Relative));

            if (sr == null)
            {
                throw new FileNotFoundException(FileName);
            }

            return sr.Stream;
        }

        public override void Dispose() { }
    }
}
