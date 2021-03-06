﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Location;
using Albite.Reader.Engine.Layout;
using Albite.Reader.App.View.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace Albite.Reader.App.View.Pages
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        public ReaderPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
        }

        private ReaderControl ReaderControl = null;

#region ApplicationBar & SystemTray
        private void ApplicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            SystemTray.IsVisible = e.IsMenuVisible;

            if (ReaderControl != null)
            {
                ReaderControl.StatusBarShown = e.IsMenuVisible;
            }

            if (e.IsMenuVisible)
            {
                // Update the buttons
                updateApplicationBarButtons();
            }
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

        private void updateApplicationBarButtons()
        {
            BookPresenter bookPresenter = App.Context.BookPresenter;

            // Should the back button be enabled?
            BackButton.IsEnabled = bookPresenter.HistoryStack.HasHistory;

            // Should the Pin-To-Start button be enabled?
            PinButton.IsEnabled = !TileManager.IsPinned(bookPresenter.Book);
        }

        private void updateColors()
        {
            // Get current layout settings
            LayoutSettings settings = App.Context.LayoutSettings;

            // Get current theme
            Theme theme = settings.Theme;

            // Set colors of the system tray
            SystemTray.BackgroundColor = theme.BackgroundColor;
            SystemTray.ForegroundColor = theme.TextColor;

            // Now set-up the application bar
            ApplicationBar.BackgroundColor = theme.BackgroundColor;
            ApplicationBar.ForegroundColor = theme.TextColor;

            // And the wait control
            WaitControl.BackgroundColor = theme.BackgroundColor;
            WaitControl.ForegroundColor = theme.TextColor;
        }
#endregion

#region ApplicationBar Buttons
        private void InitializeApplicationBar()
        {
            // The buttons can't be addressed using "x:Name", see this:
            // http://stackoverflow.com/questions/5933109/applicationbar-is-always-null
            //
            // So we need to do it the ugly way...

            // First, the icon buttons
            BackButton              = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            SettingsButton          = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            ContentsButton          = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
            AddBookmarkButton       = ApplicationBar.Buttons[3] as ApplicationBarIconButton;

            // Then the menu buttons
            ReadingPositionButton   = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            BookmarksButton         = ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
            NarrateButton           = ApplicationBar.MenuItems[2] as ApplicationBarMenuItem;
            SearchButton            = ApplicationBar.MenuItems[3] as ApplicationBarMenuItem;
            PinButton               = ApplicationBar.MenuItems[4] as ApplicationBarMenuItem;
            ShareButton             = ApplicationBar.MenuItems[5] as ApplicationBarMenuItem;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            HistoryStack historyStack = App.Context.BookPresenter.HistoryStack;

            if (historyStack.HasHistory)
            {
                // Not empty
                BookLocation previousLocation = historyStack.GetPreviousLocation();

                if (!historyStack.HasHistory)
                {
                    // No more locations, so disable the button
                    BackButton.IsEnabled = false;
                }

                // Now go to the location
                ReaderControl.BookLocation = previousLocation;
            }
        }

        private void navigate(string path)
        {
            WaitControl.Start();
            WaitControl.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/" + path, UriKind.Relative));
            });
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            navigate("BookSettings/BookSettingsPage.xaml");
        }

        private void ContentsButton_Click(object sender, EventArgs e)
        {
            navigate("ContentsPage.xaml");
        }

        private void AddBookmarkButton_Click(object sender, EventArgs e)
        {
            // Create the bookmark
            Bookmark bookmark = ReaderControl.CreateBookmark();

            // Inform the user
            MessageBox.Show(
                "Bookmark added successfully",
                "Bookmark added",
                MessageBoxButton.OK);
        }

        private void ReadingPositionButton_Click(object sender, EventArgs e)
        {
            navigate("ReadingLocationPage.xaml");
        }

        private void BookmarksButton_Click(object sender, EventArgs e)
        {
            navigate("BookmarksPage.xaml");
        }

        private void ShareButton_Click(object sender, EventArgs e)
        {
            // Get the book
            Book book = ReaderControl.BookPresenter.Book;

            // Create the share status task
            ShareStatusTask task = new ShareStatusTask();

            // Set the status message
            task.Status = string.Format(
                "I'm reading \"{0}\" by {1}.",
                book.Title,
                book.Author);

            // Now show the task
            task.Show();
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            // Pin book tile to start
            TileManager.PinBook(App.Context.BookPresenter);
        }

        private void NarrateButton_Click(object sender, EventArgs e)
        {
            navigate("Narration/NarrationPage.xaml");
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            navigate("BookSearchPage.xaml");
        }
#endregion

#region Open/persist book on load/navigating from
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (ReaderControl != null)
            {
                // TODO: Sometimes ReaderControl is null here. Why?
                BookPresenter bookPresenter = ReaderControl.BookPresenter;

                // ReaderControl.BookPresenter can be null if the user navigates
                // from *before* it has actually loaded
                if (bookPresenter != null)
                {
                    // Update the BookLocation of BookPresenter to match
                    // the current BookLocation
                    bookPresenter.HistoryStack.SetCurrentLocation(ReaderControl.BookLocation);

                    // Now persist BookPresenter
                    bookPresenter.Persist();
                }
            }

            // Show the wait control so that there would be no glitches,
            // e.g. flash of a blank web-page, etc.
            WaitControl.Start();

            // Remove the ReaderControl
            ReaderControl = null;
            ReaderControlGrid.Children.Clear();

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Update sys tray & app bar colors
            updateColors();

            // Get the book id from the query string
            int bookId = int.Parse(NavigationContext.QueryString["id"]);

            // Open the book
            BookPresenter bookPresenter = App.Context.OpenBook(bookId);

            // Disable the buttons until enabled by the client
            BackButton.IsEnabled = false;
            PinButton.IsEnabled = false;

            // Show the wait control so that there would be no glitches,
            // e.g. flash of a blank web-page, etc.
            WaitControl.Start();

            // Add the ReaderControl
            ReaderControl = new ReaderControl();
            ReaderControl.Observer = new Observer(this);
            ReaderControl.Loaded += ReaderControl_Loaded;
            ReaderControlGrid.Children.Add(ReaderControl);

            base.OnNavigatedTo(e);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Now, open the book in the control
            if (ReaderControl != null)
            {
                // ReaderControl *can* be null if ReaderControl_Loaded was
                // called after OnNavigatingFrom(). Sounds unlikely
                // but it's possible...
                ReaderControl.BookPresenter = App.Context.BookPresenter;
            }
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

            public bool OnExternalNavigationRequested(Uri uri, string title)
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

                // The UI doesn't handle relative uris
                return false;
            }

            public bool OnInternalNavigationApprovalRequested(Uri uri, string title)
            {
                if (title == null || title == string.Empty)
                {
                    title = "this link";
                }

                return MessageBox.Show(
                    string.Format("Jump to {0}?", title),
                    "Book navigation",
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }

            public void OnNavigationFailed(Uri uri, string title)
            {
                if (title == null || title == string.Empty)
                {
                    title = "this link";
                }

                MessageBox.Show(
                    string.Format("Failed to navigate to {0}", title),
                    "Navigation failed",
                    MessageBoxButton.OK);
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