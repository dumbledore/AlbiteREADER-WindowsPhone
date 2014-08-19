using System;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace Albite.Reader.App
{
    public class CustomUriMapper : UriMapperBase
    {
        private UriMapper mapper = new UriMapper();

        public Collection<UriMapping> UriMappings
        {
            get { return mapper.UriMappings; }
        }

        private static readonly Uri addBookUri = new Uri("/Albite.Reader.App;component/Source/View/Pages/AddBookPage.xaml", UriKind.Relative);

        public override Uri MapUri(Uri uri)
        {
            Uri mappedUri;

            string uriString = uri.ToString();

            if (uriString.StartsWith("/FileTypeAssociation"))
            {
                // prepare book for adding
                prepareEpub(uriString);

                // default to add book page
                mappedUri = addBookUri;
            }
            else
            {
                mappedUri = mapper.MapUri(uri);
            }

            return mappedUri;
        }

        private void prepareEpub(string uriString)
        {
            // Get the file ID (after "fileToken=").
            string tokenString = "fileToken=";
            int fileTokenIndex = uriString.IndexOf(tokenString) + tokenString.Length;
            string fileToken = uriString.Substring(fileTokenIndex);

            // Set the fileToken
            App.Context.FileToken = fileToken;
        }
    }
}
