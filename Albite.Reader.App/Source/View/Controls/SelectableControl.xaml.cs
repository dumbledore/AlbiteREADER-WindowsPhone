using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Albite.Reader.App.View.Controls
{
    public partial class SelectableControl : UserControl
    {
        public SelectableControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SelectableControl),
            new PropertyMetadata(onTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void onTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableControl control = (SelectableControl)d;
            string newValue = (string)e.NewValue;
            control.ForegroundText.Text = newValue;
        }

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(bool), typeof(SelectableControl),
            new PropertyMetadata(onSelectedChanged));

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static void onSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectableControl control = (SelectableControl)d;
            bool newValue = (bool)e.NewValue;
            Brush brush = null;
            if (newValue)
            {
                brush = (Brush)Application.Current.Resources["PhoneAccentBrush"];
            }

            control.SelectionBorder.Background = brush;
        }
    }
}
