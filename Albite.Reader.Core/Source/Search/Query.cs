using System;
using System.Collections.Generic;
using System.Linq;

namespace Albite.Reader.Core.Search
{
    public class Query
    {
        public static int MinimumTokenLength = 3;

        private string originalQuery;
        private string[] tokens;


        // It appears that the calls to char.Is*() are incredibly
        // slow, so getting these chars from there is not an option.
        // Of course we might miss some chars, but there's no better way.
        private static char[] Separators =
        {
            ' ', '\t', '\n', '\r',
            ':', ';', '!', '?', '.', ',',
            '\'', '"', '`',
            '#', '$', '%', '^', '&', '@',
             '(', ')', '[', ']', '{', '}',
            '-', '_', '=', '+', '/', '\\', '*',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
        };

        public Query(string query)
        {
            originalQuery = query;
            tokens = createTokens(query);
        }

        private static string[] createTokens(string query)
        {
            return query.Split(Separators).Where(x => x.Length >= MinimumTokenLength).ToArray();
        }

        /// <summary>
        /// Search for a query in a string
        /// </summary>
        /// <param name="haystack">String to search in</param>
        /// <returns>
        /// A value between 0 and 1, where 0 means no match,
        /// and 1 means that the whole query string was found in the haystack.
        /// </returns>
        public double Matches(string haystack)
        {
            // First try for the whole "phrase"
            if (containsToken(originalQuery, haystack))
            {
                return 1;
            }

            // Have to search in tokens
            int tokensFound = tokens.Count(x => containsToken(x, haystack));

            // Return a normalized value
            return ((double)tokensFound) / tokens.Length;
        }

        private bool containsToken(string token, string haystack)
        {
            return haystack.IndexOf(token, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
