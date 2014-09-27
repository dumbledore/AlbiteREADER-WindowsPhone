using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.BookLibrary.Search
{
    public interface IBookSeeker
    {
        /// <summary>
        /// Called every time a bunch of results is found,
        /// e.g. after searching a chapter.
        /// </summary>
        event EventHandler<IEnumerable<IBookmark>> ResultsFound;

        /// <summary>
        /// Initiates an asynchronious search in the book
        /// </summary>
        /// <param name="query">string to search for</param>
        /// <param name="cancelToken">token used to cancel the task</param>
        /// <returns>A task for all found results</returns>
        Task<IEnumerable<IBookmark>> SearchAsync(string query, CancellationToken cancelToken);

        /// <summary>
        /// Initiates a synchronious search
        /// <param name="query">string to search for</param>
        /// <returns>All found results</returns>
        /// </summary>
        IEnumerable<IBookmark> Search(string query);
    }
}
