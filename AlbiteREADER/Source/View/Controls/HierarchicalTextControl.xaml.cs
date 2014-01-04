using System;
using System.Windows;
using System.Windows.Controls;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public partial class HierarchicalTextControl : UserControl
    {
        public HierarchicalTextControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register("Text", typeof(string), typeof(HierarchicalTextControl),
            new PropertyMetadata(onTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void onTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HierarchicalTextControl control = (HierarchicalTextControl)d;
            string newValue = (string)e.NewValue;
            control.TextBlock.Text = newValue;
        }

        public static readonly int LevelOffset = 48;
        public static readonly int MaxLevel = 4;

        public static readonly DependencyProperty LevelProperty
            = DependencyProperty.Register("Level", typeof(int), typeof(HierarchicalTextControl),
            new PropertyMetadata(onLevelChanged));

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        private static void onLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HierarchicalTextControl control = (HierarchicalTextControl)d;
            int level = (int)e.NewValue;

            // Clamp in [0, MaxLevel]
            level = Math.Max(Math.Min(level, MaxLevel), 0);

            Thickness currentMargin = control.TextBlock.Margin;

            Thickness newMargin = new Thickness(
                level * LevelOffset,
                currentMargin.Top,
                currentMargin.Right,
                currentMargin.Bottom);

            control.TextBlock.Margin = newMargin;
        }
    }
}
