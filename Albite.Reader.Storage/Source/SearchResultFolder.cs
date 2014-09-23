using System.Runtime.Serialization;

namespace Albite.Reader.Storage
{
    [DataContract]
    internal class SearchResultFolder : StorageItem, ISearchResultFolder
    {
        public SearchResultFolder(string id, string name) : base(id, name) { }
    }
}
