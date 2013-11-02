using SvetlinAnkov.Albite.BookLibrary;
using System;

namespace SvetlinAnkov.Albite.Engine
{
    public interface IEngineController
    {
        /// <summary>
        /// The width of the viewport of the control
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the viewport of the control
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Base path of the browser
        /// </summary>
        string BasePath { get; set; }

        /// <summary>
        /// Reloads the browser
        /// </summary>
        void ReloadBrowser();

        /// <summary>
        /// Send message to the JS client.
        /// </summary>
        /// <param name="message">JSON-encoded message</param>
        /// <returns></returns>
        string SendMessage(string message);

        /// <summary>
        /// Indicates if the control is showing a loading screen
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Inform the control what is the current progress
        /// </summary>
        /// <param name="progress">Progress in 0..100</param>
        void LoadingProgressed(int progress);

        /// <summary>
        /// Inform the control that loading has completed
        /// </summary>
        void LoadingCompleted();

        /// <summary>
        /// Inform the control in case of an error.
        /// E.g., this might happen if the data from the client is
        /// malformed, or if the client has generated an error itself.
        /// </summary>
        /// <param name="message">
        /// An error message indicating the source of the error.
        /// </param>
        void OnError(string message);
    }
}
