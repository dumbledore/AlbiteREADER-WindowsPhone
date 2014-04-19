using System;
using Albite.Reader.Core.Collections;
using System.Windows.Media.Imaging;

namespace Albite.Reader.App
{
    public class CachedResourceImage : CachedObject<BitmapImage>
    {
        public Uri Uri { get; private set; }

        public CachedResourceImage(string path)
            : this(new Uri(path, UriKind.Relative)) { }

        public CachedResourceImage(Uri uri)
        {
            Uri = uri;
        }

        protected override BitmapImage GenerateValue()
        {
            return new BitmapImage(Uri);
        }
    }
}
