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
    }
}
