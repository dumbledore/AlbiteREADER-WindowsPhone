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
        /// The padding taken by the ApplicationBar (if any).
        /// </summary>
        int ApplicationBarHeight { get; }

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
        /// Inform the control that loading has started
        /// </summary>
        void LoadingStarted();

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
