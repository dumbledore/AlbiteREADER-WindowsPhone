using Albite.Reader.BookLibrary.Location;
using System;

namespace Albite.Reader.App.View.Controls
{
    public interface IReaderControlObserver
    {
        void OnError(string message);
        void OnContentLoadingStarted();
        void OnContentLoadingCompleted();
        bool OnNavigationRequested(Uri uri);
        int ApplicationBarHeight { get; }
    }
}
