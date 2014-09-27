using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Albite.Reader.BookLibrary.Search
{
    internal class XhtmlBookSeeker : IBookSeeker
    {
        public event EventHandler<IEnumerable<IBookmark>> ResultsFound;

        private BookPresenter presenter;

        public XhtmlBookSeeker(BookPresenter presenter)
        {
            this.presenter = presenter;
        }

        public Task<IEnumerable<IBookmark>> SearchAsync(string query, CancellationToken cancelToken)
        {
            return Task<bool>.Run(() =>
            {
                return search(query, cancelToken);
            }, cancelToken);
        }

        public IEnumerable<IBookmark> Search(string query)
        {
            return search(query, CancellationToken.None);
        }

        private IEnumerable<IBookmark> search(string query, CancellationToken cancelToken)
        {
            Searcher searcher = new Searcher(query, this, cancelToken);

            foreach (Chapter chapter in presenter.Spine)
            {
                searcher.SearchChapter(chapter);
            }

            return searcher.Results;
        }

        private class Searcher
        {
            private string query;
            private XhtmlBookSeeker seeker;
            private CancellationToken cancelToken;

            private List<IBookmark> results_ = new List<IBookmark>();
            public IEnumerable<IBookmark> Results
            {
                get { return results_; }
            }

            public Searcher(string query, XhtmlBookSeeker seeker, CancellationToken cancelToken)
            {
                this.query = query;
                this.seeker = seeker;
                this.cancelToken = cancelToken;
            }

            public void SearchChapter(Chapter chapter)
            {
                // Get a parser for this chapter
                XhtmlParser parser = new XhtmlParser(query, chapter, cancelToken);

                // Get a list of bookmark results
                IEnumerable<IBookmark> chapterResults = parser.Parse();

                // Notify if needed
                if (seeker.ResultsFound != null)
                {
                    seeker.ResultsFound(seeker, chapterResults);
                }

                // Add bunch to all results
                results_.AddRange(chapterResults);
            }
        }
    }
}
