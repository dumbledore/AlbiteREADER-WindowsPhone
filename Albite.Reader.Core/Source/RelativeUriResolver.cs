using System;

namespace Albite.Reader.Core
{
    public class RelativeUriResolver
    {
        private static readonly Uri rootUri = new Uri("file:///");

        public Uri Base { get; private set; }

        public RelativeUriResolver(string baseUri)
        {
            Base = new Uri(rootUri, baseUri);
        }

        /// <summary>
        /// Returns the URI of the resource relative to the base URI
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        public Uri Reslove(string uri)
        {
            return Resolve(Base, uri);
        }

        public Uri Resolve(Uri uri)
        {
            return Resolve(Base, uri);
        }

        /// <summary>
        /// Returns the unescaped URI of the resource relative to the base URI
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        public string ResolveToString(string uri)
        {
            return ResolveToString(Base, uri);
        }

        public string ResolveToString(Uri uri)
        {
            return ResolveToString(Base, uri);
        }

        public static Uri Resolve(Uri baseUri, string uri)
        {
            Uri tempUri = new Uri(baseUri, uri);
            return rootUri.MakeRelativeUri(tempUri);
        }

        public static Uri Resolve(Uri baseUri, Uri uri)
        {
            Uri tempUri = new Uri(baseUri, uri);
            return rootUri.MakeRelativeUri(tempUri);
        }

        public static string ResolveToString(Uri baseUri, string uri)
        {
            return Uri.UnescapeDataString(Resolve(baseUri, uri).ToString());
        }

        public static string ResolveToString(Uri baseUri, Uri uri)
        {
            return Uri.UnescapeDataString(Resolve(baseUri, uri).ToString());
        }
    }
}
