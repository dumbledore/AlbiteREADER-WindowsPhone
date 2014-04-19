using Albite.Reader.App.Browse;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Albite.Reader.App.View.Controls
{
    public class BrowsingServiceControl : IconContentControl
    {
        public BrowsingServiceControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BrowsingServiceProperty
            = DependencyProperty.Register("BrowsingService", typeof(IBrowsingService), typeof(BrowsingServiceControl),
            new PropertyMetadata(onBookmarkChanged));

        public IBrowsingService BrowsingService
        {
            get { return (IBrowsingService)GetValue(BrowsingServiceProperty); }
            set { SetValue(BrowsingServiceProperty, value); }
        }

        private static void onBookmarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrowsingServiceControl control = (BrowsingServiceControl)d;

            IBrowsingService newValue = (IBrowsingService)e.NewValue;

            control.ContentTextBlock.Text = newValue.Name;
            control.Icon.Source = new BitmapImage(
                new Uri("/Resources/Images/onedrive.png", UriKind.Relative));
        }
    }

}
