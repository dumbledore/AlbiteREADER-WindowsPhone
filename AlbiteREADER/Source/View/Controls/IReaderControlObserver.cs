namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public interface IReaderControlObserver
    {
        void OnError(string message);
        void OnContentLoadingStarted();
        void OnContentLoadingCompleted();
        bool OnNavigationRequested(string url);
        int ApplicationBarHeight { get; }
    }
}
