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
using System.Windows.Resources;
using System.IO;
using SvetlinAnkov.AlbiteREADER.Utils;

namespace SvetlinAnkov.AlbiteREADER.Utils
{
    public class AlbiteResourceStorage : AlbiteStorage
    {
        private static readonly string ResourcesLocation = Defaults.Application.Resources;

        public AlbiteResourceStorage(string filename) : base(filename) { }

        public override AlbiteStorage OpenRelative(string filename)
        {
            return new AlbiteResourceStorage(this.filename + filename);
        }

        protected override Stream getStream(FileMode fileMode)
        {
            StreamResourceInfo sr = Application.GetResourceStream(new Uri(ResourcesLocation + filename, UriKind.Relative));

            if (sr == null)
            {
                throw new InvalidOperationException("File " + filename + " not found in Content!");
            }

            return sr.Stream;
        }

        protected override void CreatePathForFile()
        {
            throw new InvalidOperationException("Writing is not available for Resources");
        }

        public override void Dispose() { }
    }
}
