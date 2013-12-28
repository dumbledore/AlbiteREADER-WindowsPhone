using SvetlinAnkov.Albite.BookLibrary;
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
        /// Create a Bookmark for the current BookLocation.
        ///
        /// Note that this will get the bookmark from the client,
        /// and then *create* a new record in the library database
        /// if there's no bookmark with the same location. If there
        /// is, it will return the previously created entity.
        /// </summary>
        Bookmark CreateBookmark();

        /// <summary>
        /// Current page
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// Number of pages in this chapter
        /// </summary>
        int PageCount { get; }

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
