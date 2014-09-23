using Albite.Reader.Core.Search;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.Storage.Services
{
    public abstract class SearchableStorageService : StorageService
    {
        public override bool IsSearchSupported
        {
            get { return true; }
        }

        public override ISearchResultFolder GetSearchFolder(string query)
        {
            return new SearchResultFolder(query, query);
        }

        protected async Task<ICollection<IStorageItem>> Search(string query, CancellationToken ct)
        {
            // List of results
            List<SearchResult> results = new List<SearchResult>();

            // A stack of folders to search. This way
            // we are not using recursion, which is dangerous
            // when going through folders, as one never knows
            // how deep the recursion would go.
            Stack<IStorageFolder> foldersToSearch = new Stack<IStorageFolder>();

            // Add the root folder
            foldersToSearch.Push(null);

            // Query for matching
            Query q = new Query(query);

            while (foldersToSearch.Count > 0)
            {
                // Have we been cancelled?
                ct.ThrowIfCancellationRequested();

                // Get a folder to search in
                IStorageFolder folder = foldersToSearch.Pop();

                // Wait for results
                await searchFolder(folder, foldersToSearch, q, results, ct);
            }

            // Sort the results
            results.Sort();

            // Now convert the results to ICollection<StorageItem>
            IStorageItem[] items = new IStorageItem[results.Count];

            // Not using a for() with an indexer for results, as for
            // linked lists it's faster to use an iterator...
            int i = 0;
            foreach (SearchResult result in results)
            {
                items[i++] = result.Item;
            }

            return items;
        }

        private async Task searchFolder(
            IStorageFolder folder,
            Stack<IStorageFolder> foldersToSearch,
            Query query,
            List<SearchResult> results, CancellationToken ct)
        {
            ICollection<IStorageItem> items = await GetFolderContentsAsync(folder, ct);

            foreach (IStorageItem item in items)
            {
                double score = query.Matches(item.Name);
                if (score > 0)
                {
                    // found, so add to resutls
                    results.Add(new SearchResult(score, item));
                }

                if (item is IStorageFolder)
                {
                    // Needs searching
                    foldersToSearch.Push((IStorageFolder)item);
                }
            }
        }

        private class SearchResult : IComparable<SearchResult>
        {
            public double Score { get; private set; }
            public IStorageItem Item { get; private set; }

            public SearchResult(double score, IStorageItem item)
            {
                Score = score;
                Item = item;
            }

            public int CompareTo(SearchResult other)
            {
                if (Score < other.Score)
                {
                    return -1;
                }
                else if (Score > other.Score)
                {
                    return 1;
                }
                else
                {
                    // they are equal
                    return 0;
                }
            }
        }
    }
}
