﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Core.Utils;

namespace SvetlinAnkov.Albite.READER.Model.Reader
{
    public class BookEngine : BrowserEngine
    {
        private static readonly string tag = "BookEngine";

        public BookEngine(IEngineController controller, Settings settings)
            : base(controller, settings) { }

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
                SetChapterDomLocation(value.SpineElement.Chapter, value.DomLocation);
            }
        }

        public bool IsFirstChapter
        {
            get { return current.Previous == null; }
        }

        public bool IsLastChapter
        {
            get { return current.Next == null; }
        }

        public void GoToPreviousChapter()
        {
            if (IsFirstChapter)
            {
                Log.W(tag, "It's the first chapter already");
                return;
            }

            current = current.Previous;
            SetChapterLastPage(current.Chapter);
        }

        public void GoToNextChapter()
        {
            if (IsLastChapter)
            {
                Log.W(tag, "It's the last chapter already");
                return;
            }

            current = current.Next;
            SetChapterFirstPage(current.Chapter);
        }

        // Private API
    }
}
