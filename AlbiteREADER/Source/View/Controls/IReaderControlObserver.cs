using SvetlinAnkov.Albite.BookLibrary.Location;
using System;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public interface IReaderControlObserver
    {
        void OnError(string message);
        void OnContentLoadingStarted();
        void OnContentLoadingCompleted();
        bool OnNavigationRequested(Uri uri);
        void OnNavigating(BookLocation currentLocation);
        int ApplicationBarHeight { get; }
    }
}
