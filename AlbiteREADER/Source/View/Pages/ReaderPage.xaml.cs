using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SvetlinAnkov.Albite.BookLibrary;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                // TODO: Clear the history stack if it's there
            }
            base.OnNavigatedTo(e);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the book id from the query string
            int bookId = int.Parse(NavigationContext.QueryString["id"]);

            // Get the library from the current context
            Library library = App.Context.Library;

            // Get the book for the given id
            Book book = library.Books[bookId];

            // Now, open the book in the control
            ReaderControl.OpenBook(book);
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

            public bool OnNavigationRequested(Uri uri)
            {
                if (uri.IsAbsoluteUri)
                {
                    if (uri.Scheme == Uri.UriSchemeHttp
                        || uri.Scheme == Uri.UriSchemeHttps)
                    {
                        // A web page
                        if (MessageBox.Show(
                            "Would you like to open the link in the web browser?",
                            "External link",
                            MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            // Launch the web browser
                            WebBrowserTask task = new WebBrowserTask();
                            task.Uri = uri;
                            task.Show();
                        }
                    }
                    else if (uri.Scheme == Uri.UriSchemeMailto)
                    {
                        string mailAddress = string.Format("{0}@{1}", uri.UserInfo, uri.Host);

                        // An email address
                        if (MessageBox.Show(
                            "Would you like to compose an email to " + mailAddress + "?",
                            "E-mail address",
                            MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            EmailComposeTask task = new EmailComposeTask();
                            task.To = mailAddress;
                            task.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "This scheme is not supported: " + uri.Scheme,
                            "Unknown scheme",
                            MessageBoxButton.OK);
                    }
                    return true;
                }

                // The UI doesn't handle internal jumps
                return false;
            }

            public int ApplicationBarHeight
            {
                get
                {
                    return page.shouldShowApplicationBar(page.Orientation)
                        ? (int) page.ApplicationBar.MiniSize : 0;
                }
            }
        }
#endregion
    }
}