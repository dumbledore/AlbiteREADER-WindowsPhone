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

        public override Uri MapUri(Uri uri)
        {
            return mapper.MapUri(uri);
        }
    }
}
