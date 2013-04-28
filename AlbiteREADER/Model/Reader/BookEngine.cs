﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.READER.Model.Reader.Browser;

namespace SvetlinAnkov.Albite.READER.Model
{
    public class BookEngine
    {
        private readonly WebBrowser webBrowser;
        private readonly Book.Presenter presenter;
        private BrowserEngine engine;

        public BookEngine(WebBrowser webBrowser, Book.Presenter presenter, Settings settings)
        {
            this.webBrowser = webBrowser;
            this.presenter = presenter;
            this.engine = new BrowserEngine(webBrowser, presenter, settings);
        }

        // TODO: Add history stack

        Book.SpineElement current;

        /// <summary>
        /// Setting this property would cause the engine
        /// to load into the specified book loacation
        /// </summary>
        public Book.BookLocation BookLocation
        {
            get
            {
                // TODO: What about when the current spine element
                // is not the current chapter, and thus is not
                return null;
            }

            set
            {
                current = value.SpineElement;
                engine.Chapter = value.SpineElement.Chapter;
                engine.DomLocation = value.DomLocation;
            }
        }

        // Private API
    }
}