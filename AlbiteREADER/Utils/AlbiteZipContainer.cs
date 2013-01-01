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
using System.Windows.Resources;
using System.Diagnostics;

namespace SvetlinAnkov.AlbiteREADER.Utils
{
    public class AlbiteZipContainer : IAlbiteContainer
    {
        private readonly Stream stream;

        public AlbiteZipContainer(Stream stream)
        {
            if (stream == null)
            {
                throw new NullReferenceException("Stream must not be null");
            }

            this.stream = stream;
        }

        public Stream Stream(string entityName)
        {
            Uri uri = new Uri(entityName, UriKind.Relative);
            StreamResourceInfo info = new StreamResourceInfo(this.stream, null);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(info, uri);

            return stream.Stream;
        }

        public void UnzipStorage(string entityName, AlbiteStorage output)
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
