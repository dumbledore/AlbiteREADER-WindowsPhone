using Albite.Reader.BookLibrary.Location;
using System;

namespace Albite.Reader.App.View.Controls
{
    public interface IReaderControlObserver
    {
        void OnError(string message);
        void OnContentLoadingStarted();
        void OnContentLoadingCompleted();
        bool OnExternalNavigationRequested(Uri uri, string title);
        bool OnInternalNavigationApprovalRequested(Uri uri, string title);
        void OnNavigationFailed(Uri uri, string title);

        int ApplicationBarHeight { get; }
    }
}
