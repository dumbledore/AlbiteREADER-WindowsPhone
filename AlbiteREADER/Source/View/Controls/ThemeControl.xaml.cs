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

        public static readonly DependencyProperty ThemeBackgroundColorProperty
            = DependencyProperty.Register("ThemeBackgroundColor", typeof(Color), typeof(ThemeControl),
            new PropertyMetadata(onThemeBackgroundColorChanged));

        public Color ThemeBackgroundColor
        {
            get { return (Color)GetValue(ThemeBackgroundColorProperty); }
            set { SetValue(ThemeBackgroundColorProperty, value); }
        }

        public static void onThemeBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            Color newValue = (Color)e.NewValue;
            control.BackgroundBorder.Background = new SolidColorBrush(newValue);
        }

        public static readonly DependencyProperty ThemeForegroundColorProperty
            = DependencyProperty.Register("ThemeForegroundColor", typeof(Color), typeof(ThemeControl),
            new PropertyMetadata(onThemeForegroundColorChanged));

        public Color ThemeForegroundColor
        {
            get { return (Color)GetValue(ThemeForegroundColorProperty); }
            set { SetValue(ThemeForegroundColorProperty, value); }
        }

        public static void onThemeForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            Color newValue = (Color)e.NewValue;
            SolidColorBrush colorBrush = new SolidColorBrush(newValue);
            control.ForegroundText.Foreground = colorBrush;
            control.BackgroundBorder.BorderBrush = colorBrush;
        }

        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(ThemeControl),
            new PropertyMetadata(onHeaderTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void onHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemeControl control = (ThemeControl)d;
            string newValue = (string)e.NewValue;
            control.ForegroundText.Text = newValue;
        }
    }
}
