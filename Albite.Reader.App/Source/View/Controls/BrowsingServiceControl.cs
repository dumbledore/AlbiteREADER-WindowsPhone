using Albite.Reader.Storage;
using System.Windows;

namespace Albite.Reader.App.View.Controls
{
    public class BrowsingServiceControl : IconContentControl
    {
        public BrowsingServiceControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BrowsingServiceProperty
            = DependencyProperty.Register("BrowsingService", typeof(StorageService), typeof(BrowsingServiceControl),
            new PropertyMetadata(onBookmarkChanged));

        public StorageService BrowsingService
        {
            get { return (StorageService)GetValue(BrowsingServiceProperty); }
            set { SetValue(BrowsingServiceProperty, value); }
        }

        private static void onBookmarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrowsingServiceControl control = (BrowsingServiceControl)d;

            StorageService newValue = (StorageService)e.NewValue;

            control.ContentTextBlock.Text = newValue.Name;
            control.Icon.Source = newValue.Icon;
        }
    }
}
