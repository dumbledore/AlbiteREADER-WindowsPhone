using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        public ReaderPage()
        {
            InitializeComponent();

            ReaderControl.Observer = new Observer(this);
        }

#region ApplicationBar & SystemTray
        private void ApplicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            SystemTray.IsVisible = e.IsMenuVisible;
        }

        private bool shouldShowApplicationBar(PageOrientation orientation)
        {
            return orientation == PageOrientation.PortraitUp;
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            ApplicationBar.IsVisible = shouldShowApplicationBar(e.Orientation);
            base.OnOrientationChanged(e);
        }
#endregion

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e) { }

#region Open/persist book on load/navigating from
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ReaderControl.PersistBook();
            base.OnNavigatingFrom(e);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the book id from the query string
            int bookId = int.Parse(NavigationContext.QueryString["id"]);

            // Now, open the book in the control
            ReaderControl.OpenBook(bookId);
        }
#endregion

#region IReaderContenObserver
        private class Observer : IReaderControlObserver
        {
            private ReaderPage page;

            public Observer(ReaderPage page)
            {
                this.page = page;
            }

            public void OnError(string message)
            {
                MessageBox.Show("There was a problem with this book:\n" + message);

                // There MUST be a previous entry, otherwise it wouldn't
                // make sense
                page.NavigationService.GoBack();
            }

            public void OnContentLoadingStarted()
            {
                // Hide the bar when loading
                page.ApplicationBar.IsVisible = false;

                page.WaitControl.Start();
            }

            public void OnContentLoadingCompleted()
            {
                page.WaitControl.Finish();

                // Show the bar if adequate
                page.ApplicationBar.IsVisible = page.shouldShowApplicationBar(page.Orientation);
            }
        }
    }
#endregion
}