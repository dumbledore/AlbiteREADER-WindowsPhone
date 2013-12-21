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
    public partial class ThemeControl : UserControl
    {
        public ThemeControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BackgroundColorProperty
            = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(ThemeControl),
            new PropertyMetadata(onBackgroundColorChanged));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static void onBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            Color newValue = (Color)e.NewValue;
            control.BackgroundBorder.Background = new SolidColorBrush(newValue);
        }

        public static readonly DependencyProperty ForegroundColorProperty
            = DependencyProperty.Register("ForegroundColor", typeof(Color), typeof(ThemeControl),
            new PropertyMetadata(onForegroundColorChanged));

        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public static void onForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            Color newValue = (Color)e.NewValue;
            SolidColorBrush colorBrush = new SolidColorBrush(newValue);
            control.ForegroundText.Foreground = colorBrush;
            control.BackgroundBorder.BorderBrush = colorBrush;
        }

        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(ThemeControl),
            new PropertyMetadata(onTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void onTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            string newValue = (string)e.NewValue;
            control.ForegroundText.Text = newValue;
        }

        public static readonly DependencyProperty SelectedProperty
            = DependencyProperty.Register("Selected", typeof(bool), typeof(ThemeControl),
            new PropertyMetadata(onSelectedChanged));

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static void onSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            bool newValue = (bool)e.NewValue;
            Brush brush = null;
            if (newValue)
            {
                brush = (Brush)Application.Current.Resources["PhoneAccentBrush"];
            }

            control.SelectionBorder.Background = brush;
        }

        public new static readonly DependencyProperty FontFamilyProperty
            = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(ThemeControl),
            new PropertyMetadata(onFontFamilyChanged));

        public new FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static void onFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            FontFamily family = (FontFamily)e.NewValue;
            control.ForegroundText.FontFamily = family;
        }

        public new static readonly DependencyProperty FontSizeProperty
            = DependencyProperty.Register("FontSize", typeof(double), typeof(ThemeControl),
            new PropertyMetadata(onFontSizeChanged));

        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static void onFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            double fontSize = (double)e.NewValue;
            control.ForegroundText.FontSize = fontSize;
        }
    }
}
