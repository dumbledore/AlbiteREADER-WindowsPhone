using System.Windows;
using System.Windows.Media;

namespace Albite.Reader.App.View.Controls
{
    public class ThemeControl : SelectableControl
    {
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(SelectableControl),
            new PropertyMetadata(onBackgroundColorChanged));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static void onBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableControl control = (SelectableControl)d;
            Color newValue = (Color)e.NewValue;
            control.BackgroundBorder.Background = new SolidColorBrush(newValue);
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Color), typeof(SelectableControl),
            new PropertyMetadata(onForegroundColorChanged));

        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public static void onForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableControl control = (SelectableControl)d;
            Color newValue = (Color)e.NewValue;
            SolidColorBrush colorBrush = new SolidColorBrush(newValue);
            control.ForegroundText.Foreground = colorBrush;
            control.BackgroundBorder.BorderBrush = colorBrush;
        }

        public new static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(SelectableControl),
            new PropertyMetadata(onFontFamilyChanged));

        public new FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static void onFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableControl control = (SelectableControl)d;
            FontFamily family = (FontFamily)e.NewValue;
            control.ForegroundText.FontFamily = family;
        }

        // Font size is in logical pixels.
        // See http://www.silverlightshow.net/items/Windows-Phone-8-Multiple-Screen-Resolutions.aspx
        public new static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(SelectableControl),
            new PropertyMetadata(onFontSizeChanged));

        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static void onFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableControl control = (SelectableControl)d;
            double fontSize = (double)e.NewValue;
            control.ForegroundText.FontSize = fontSize;
        }

        public static readonly DependencyProperty LineSpacingProperty =
            DependencyProperty.Register("LineSpacing", typeof(double), typeof(SelectableControl),
            new PropertyMetadata(null));

        public double LineSpacing
        {
            get { return (double)GetValue(LineSpacingProperty); }
            set { SetValue(LineSpacingProperty, value); }
        }

        public static readonly DependencyProperty PageMarginsProperty =
            DependencyProperty.Register("PageMargins", typeof(double), typeof(SelectableControl),
            new PropertyMetadata(null));

        public double PageMargins
        {
            get { return (double)GetValue(PageMarginsProperty); }
            set { SetValue(PageMarginsProperty, value); }
        }
    }
}
