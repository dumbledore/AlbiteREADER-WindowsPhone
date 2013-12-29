using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Engine.Layout;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class ReaderPage : PhoneApplicationPage
    {
        public ReaderPage()
        {
            InitializeComponent();
            InitializeApplicationBar();
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

        private void updateApplicationBarButtons()
        {
            // Should the back button be enabled?
            BackButton.IsEnabled = !historyStack.IsEmpty;

            // Should the Pin-To-Start button be enabled?
            PinButton.IsEnabled = false; // TODO
        }

        private void updateColors()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get current layout settings
            LayoutSettings settings = context.LayoutSettings;

            // Get current theme
            Theme theme = settings.Theme;

            // Set colors of the system tray
            SystemTray.BackgroundColor = theme.BackgroundColor;
            SystemTray.ForegroundColor = theme.TextColor;

            // Now set-up the application bar
            ApplicationBar.BackgroundColor = theme.BackgroundColor;
            ApplicationBar.ForegroundColor = theme.TextColor;
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
            ShareButton             = ApplicationBar.MenuItems[2] as ApplicationBarMenuItem;
            PinButton               = ApplicationBar.MenuItems[3] as ApplicationBarMenuItem;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (!historyStack.IsEmpty)
            {
                // Not empty
                BookLocation previousLocation = historyStack.Pop();

                if (historyStack.IsEmpty)
                {
                    // No more locations, so disable the button
                    BackButton.IsEnabled = false;
                }

                // Now go to the location
                ReaderControl.BookLocation = previousLocation;
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/BookSettings/BookSettingsPage.xaml", UriKind.Relative));
        }

        private void ContentsButton_Click(object sender, EventArgs e)
        {
            // TODO: Navigate to Table of Contents
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
            NavigationService.Navigate(new Uri("/AlbiteREADER;component/Source/View/Pages/ReadingLocationPage.xaml", UriKind.Relative));
        }

        private void BookmarksButton_Click(object sender, EventArgs e)
        {
            // Navigate to page showing list of bookmarks
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
        }
#endregion

#region Open/persist book on load/navigating from
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            BookPresenter bookPresenter = ReaderControl.BookPresenter;

            // Update the BookLocation of BookPresenter to match
            // the current BookLocation
            bookPresenter.BookLocation = ReaderControl.BookLocation;

            // Now persist BookPresenter
            bookPresenter.Persist();

            // Try persisting the history stack
            if (e.NavigationMode != NavigationMode.Back)
            {
                // No need to persist it if the page
                // is going to be discarded
                string historyStackData = historyStack.ToString();
                State[historyStackTag] = historyStackData;
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Update sys tray & app bar colors
            updateColors();

            if (e.NavigationMode == NavigationMode.New)
            {
                // New navigation, clear the state if there was
                // anything already persisted... (Shouldn't happen)
                // Note, IDictionary<>.Remove() doesn't throw if the
                // item is not found.
                State.Remove(historyStackTag);
            }
            else
            {
                // Try to get it from the State
                if (historyStack == null && State.ContainsKey(historyStackTag))
                {
                    string historyStackData = (string)State[historyStackTag];
                    historyStack = HistoryStack.FromString(historyStackData);
                }
            }

            if (historyStack == null)
            {
                // The stack was not persisted, so create a new one
                historyStack = new HistoryStack();
            }

            base.OnNavigatedTo(e);
        }

        private void ReaderControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the book id from the query string
            int bookId = int.Parse(NavigationContext.QueryString["id"]);

            // Open the book
            BookPresenter bookPresenter = App.Context.OpenBook(bookId);

            // Now, open the book in the control
            ReaderControl.BookPresenter = bookPresenter;

            // Attach the history stack to the book presenter
            historyStack.Attach(bookPresenter);

            // Update the application bar
            updateApplicationBarButtons();
        }
#endregion

#region History Stack
        private static readonly string historyStackTag = "HistoryStack";
        private HistoryStack historyStack = null;
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

            public void OnNavigating(BookLocation currentLocation)
            {
                page.historyStack.Push(currentLocation);
                page.BackButton.IsEnabled = true;
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