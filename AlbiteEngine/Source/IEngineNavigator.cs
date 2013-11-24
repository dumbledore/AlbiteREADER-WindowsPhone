using SvetlinAnkov.Albite.BookLibrary.Location;

namespace SvetlinAnkov.Albite.Engine
{
    /// <summary>
    /// Handles navigation: DomLocations, Pages and Chapters.
    /// </summary>
    public interface IEngineNavigator
    {
        /// <summary>
        /// Current BookLocation
        /// </summary>
        BookLocation BookLocation { get; set; }

        /// <summary>
        /// Current page
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// Number of pages in this chapter
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Go to the first page in the chapter
        /// </summary>
        void GoToFirstPage();

        /// <summary>
        /// Go to the last page in the chapter
        /// </summary>
        void GoToLastPage();

        /// <summary>
        /// True, if this is the first chapter in the book
        /// </summary>
        bool IsFirstChapter { get; }

        /// <summary>
        /// True, if this is the last chapter in the book
        /// </summary>
        bool IsLastChapter { get; }

        /// <summary>
        /// Go to the previous chapter in the book
        /// </summary>
        void GoToPreviousChapter();

        /// <summary>
        /// Go to the next chapter in the book
        /// </summary>
        void GoToNextChapter();

        /// <summary>
        /// Go to a spine element
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="goToBeginning">If true, it goes to the first page of the chapter,
        /// otherwise it goes to the last</param>
        void GoToChapter(SpineElement chapter, bool goToBeginning = true);

        /// <summary>
        /// Go to a SpineElement, specifying a fragment string
        /// </summary>
        /// <param name="chapter">The chapter to go to</param>
        /// <param name="fragment">The hash string, without the hash character</param>
        void GoToChapter(SpineElement chapter, string fragment);
    }
}
