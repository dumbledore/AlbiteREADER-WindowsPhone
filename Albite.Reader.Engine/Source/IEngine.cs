﻿using Albite.Reader.BookLibrary;
using Albite.Reader.Engine.Layout;
using System;

namespace Albite.Reader.Engine
{
    public interface IEngine
    {
        /// <summary>
        /// BookPresenter for this engine
        /// </summary>
        BookPresenter BookPresenter { get; }

        /// <summary>
        /// Settings for this engine
        /// </summary>
        LayoutSettings Settings { get; }

        /// <summary>
        /// The Uri used for loading the engine.
        /// Generally, that is the main page that is used to show
        /// all chapters. It does not change.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Called whenever the viewport is resized or the dimensions
        /// might have changed (e.g. after the client has loaded).
        /// </summary>
        /// <returns>true if an update was immediately triggered</returns>
        bool UpdateDimensions();

        /// <summary>
        /// Navigator for this engine
        /// </summary>
        IEngineNavigator Navigator { get; }

        /// <summary>
        /// Indicates if the client has loaded and is safe to communicate with it
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Process a message from the client.
        /// </summary>
        /// <param name="message">JSON-encoded message from the JS client</param>
        void ReceiveMessage(string message);

        /// <summary>
        /// The name of the function that is used to contact the JSEngine
        /// </summary>
        string NotifyFunction { get; }

        bool StatusBarShown { set; }
    }
}
