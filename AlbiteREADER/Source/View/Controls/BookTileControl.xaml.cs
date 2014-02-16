using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public partial class BookTileControl : UserControl
    {
        public BookTileControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BookTitleProperty
            = DependencyProperty.Register("BookTitle", typeof(string), typeof(BookTileControl),
            new PropertyMetadata(onBookTitleChanged));

        public string BookTitle
        {
            get { return (string)GetValue(BookTitleProperty); }
            set { SetValue(BookTitleProperty, value); }
        }

        public static void onBookTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BookTileControl c = (BookTileControl)d;
            string title = (string)e.NewValue;
            c.BookTitleText.Text = title;
        }

        public static readonly DependencyProperty BookColorProperty
            = DependencyProperty.Register("BookColor", typeof(Color), typeof(BookTileControl),
            new PropertyMetadata(onBookColorChanged));

        public Color BookColor
        {
            get { return (Color)GetValue(BookColorProperty); }
            set { SetValue(BookColorProperty, value); }
        }

        public static void onBookColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BookTileControl c = (BookTileControl)d;
            Color color = (Color)e.NewValue;
            c.BookCoverGrid.Background = new SolidColorBrush(color);
        }
    }
}
