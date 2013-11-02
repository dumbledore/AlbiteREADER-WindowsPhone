using SvetlinAnkov.Albite.BookLibrary;
using System;

namespace SvetlinAnkov.Albite.Engine
{
    public interface IEngineController
    {
        int ViewportWidth { get; }
        int ViewportHeight { get; }

        string BasePath { get; set; }
        Uri SourceUri { get; set; }

        string SendMessage(string message);

        BookPresenter BookPresenter { get; }

        bool IsLoading { get; }
        void LoadingStarted();
        void LoadingProgressed(int progress);
        void LoadingCompleted();

        void OnError(string message);
    }
}
