using System;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER
{
    public class AlbiteUriMapper : UriMapperBase
    {
        private UriMapper mapper = new UriMapper();

        public Collection<UriMapping> UriMappings
        {
            get { return mapper.UriMappings; }
        }

        private static readonly Uri addBookUri = new Uri("/AlbiteREADER;component/Source/View/Pages/AddBookPage.xaml", UriKind.Relative);

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

            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Set the fileToken
            context.FileToken = fileToken;
        }
    }
}
