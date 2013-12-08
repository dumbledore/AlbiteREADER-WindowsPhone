using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public partial class HeaderedContentControl : UserControl
    {
        public HeaderedContentControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderTextProperty
            = DependencyProperty.Register("HeaderText", typeof(string), typeof(HeaderedContentControl),
            new PropertyMetadata(onHeaderTextChanged));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        private static void onHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl control = (HeaderedContentControl)d;
            string newValue = (string)e.NewValue;
            control.HeaderTextBlock.Text = newValue;
        }

        public static readonly DependencyProperty ContentTextProperty
            = DependencyProperty.Register("ContentText", typeof(string), typeof(HeaderedContentControl),
            new PropertyMetadata(onContentTextChanged));

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        private static void onContentTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl control = (HeaderedContentControl)d;
            string newValue = (string)e.NewValue;
            control.ContentTextBlock.Text = newValue;
        }

    }
}
