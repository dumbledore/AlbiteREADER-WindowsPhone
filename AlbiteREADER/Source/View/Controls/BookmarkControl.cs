﻿using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using System.Windows;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public class BookmarkControl : HeaderedContentControl
    {
        public static readonly DependencyProperty BookmarkProperty
            = DependencyProperty.Register("Bookmark", typeof(Bookmark), typeof(BookmarkControl),
            new PropertyMetadata(onBookmarkChanged));

        public Bookmark Bookmark
        {
            get { return (Bookmark)GetValue(BookmarkProperty); }
            set { SetValue(BookmarkProperty, value); }
        }

        private static void onBookmarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BookmarkControl control = (BookmarkControl)d;

            Bookmark newValue = (Bookmark)e.NewValue;

            // Set up book position as header text
            control.HeaderText = string.Format("{0:P0}", getReadingPosition(newValue));

            // Set up bookmark text as content text
            control.ContentText = newValue.Text;
        }

        private static double getReadingPosition(Bookmark bookmark)
        {
            BookLocation location = bookmark.BookLocation;

            // Total number of chapters
            int chapterCount = location.Chapter.BookPresenter.Spine.Length;

            // Each chapter gets this interval in [0, 1]
            double chapterInterval = 1 / (double)chapterCount;

            // Chapter starts at this position
            double chapterPosition = location.Chapter.Number * chapterInterval;

            // Additional offset to account for chapter location
            double chapterOffset = location.Location.RelativeLocation * chapterInterval;

            // return the total
            return chapterPosition + chapterOffset;
        }
    }
}