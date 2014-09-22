using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Albite.Reader.App.View.Controls;
using System.Windows.Input;
using Albite.Reader.App.Browse;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Albite.Reader.Storage;
using Albite.Reader.Core.Threading;
using Microsoft.Phone.Shell;
using System.Windows.Controls;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BrowsePage : PhoneApplicationPage
    {
        public BrowsePage()
        {
            InitializeComponent();
        }

        private StorageService service = null;
        private string loadingString = "";

        private FolderHistoryStack history;

        private static readonly string HistoryKey = "history";

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (State.ContainsKey(HistoryKey))
            {
                // restore history
                history = FolderHistoryStack.FromString((string)State[HistoryKey]);
            }
            else
            {
                history = new FolderHistoryStack();
            }

            // Add callback
            history.FolderChangedDelegate = loadFolderContents;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Cancel any running tasks
            cancelCurrentTask();

            // Close the search
            closeSearch();

            if (e.NavigationMode != NavigationMode.Back)
            {
                // Save the history if not going back
                State[HistoryKey] = history.ToString();
            }
            else
            {
                // Dispose the VoiceSearchControl
                SearchPanel.Dispose();
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            if (downloading)
            {
                // Cancel the download, but do not go up.
                e.Cancel = true;

                // Clear downloading flag
                downloading = false;
            }
            else if (SearchPanel.Visibility == Visibility.Visible)
            {
                // Close the search
                closeSearch();

                // Cancel the event
                e.Cancel = true;
            }
            else if (history.CanGoBack)
            {
                // Go back one step
                history.GoBack();

                // Cancel event, i.e. do not leave page
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        private CancellableTask currentTask;

        private bool downloading = false;

        private void cancelCurrentTask()
        {
            if (currentTask != null)
            {
                currentTask.Cancel();
                currentTask = null;
            }

            WaitControl.Finish();
        }

        private void showMessage(string message)
        {
            MessageBox.Show(message, service.Name, MessageBoxButton.OK);
        }

        private void FolderControl_Tap(object sender, GEArgs e)
        {
            FolderControl control = (FolderControl)sender;

            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            IStorageItem item = control.FolderItem;
            if (item is IStorageFolder)
            {
                IStorageFolder folder = (IStorageFolder)item;
                history.GoForward(folder);
            }
            else if (item is IStorageFile)
            {
                IStorageFile file = (IStorageFile)item;
                downloadFile(file);
            }
        }

        private async Task logIn()
        {
            if (service.LoginRequired)
            {
                try
                {
                    await service.LogIn();
                }
                catch (Exception e)
                {
                    string message = string.Format(
                        "Failed logging in: {0}", e.Message);
                    showMessage(message);

                    WaitControl.Finish();
                    NavigationService.GoBack();

                    // Throw back the exception so that
                    // the call chain won't go on
                    throw e;
                }
            }
        }

        private void loadFolderContents(IStorageFolder folder)
        {
            // Update the folder title
            FolderText.Text = folder == null ? service.Name : folder.Name;

            // Start the waiting control
            WaitControl.Text = loadingString;
            WaitControl.IsIndeterminate = true;
            WaitControl.Start();

            // Reset the folder contents
            FoldersList.ItemsSource = null;
            EmptyTextBlock.Visibility = Visibility.Collapsed;

            // Now create the task
            CancellationTokenSource cts = new CancellationTokenSource();
            currentTask = new CancellableTask(loadFolderContentsAsync(folder, cts.Token), cts);
        }

        private async Task loadFolderContentsAsync(IStorageFolder folder, CancellationToken ct)
        {
            await logIn();

            // Check if cancelled in the meantime
            ct.ThrowIfCancellationRequested();

            ICollection<IStorageItem> items = null;

            try
            {
                items = await service.GetFolderContentsAsync(folder, ct);
            }
            catch (Exception e)
            {
                // Skip OperationCancelExceptions
                if (!(e is OperationCanceledException))
                {
                    string message = string.Format(
                        "Failed accessing folder {0}: {1}", folder == null ? "root" : folder.Name, e.Message);
                    showMessage(message);

                    WaitControl.Finish();
                    NavigationService.GoBack();
                }

                // Throw back the exception so that
                // the call chain won't go on
                throw e;
            }

            // Not cancelled?
            ct.ThrowIfCancellationRequested();

            if (items.Count == 0)
            {
                // Empty folder
                FoldersList.ItemsSource = null;
                EmptyTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                // Has items
                FoldersList.ItemsSource = items;
                EmptyTextBlock.Visibility = Visibility.Collapsed;
            }

            WaitControl.Finish();
        }

        private void downloadFile(IStorageFile file)
        {
            // Start the waiting control
            WaitControl.Text = "Downloading " + file.Name + "...";
            WaitControl.Minimum = 0;
            WaitControl.Maximum = 100;
            WaitControl.IsIndeterminate = false;
            WaitControl.Start();

            // Set downloading flag
            downloading = true;

            // Now create the task
            CancellationTokenSource cts = new CancellationTokenSource();
            currentTask = new CancellableTask(downloadFileAsync(file, cts.Token, new Progress(this)), cts);
        }

        private async Task downloadFileAsync(
            IStorageFile file, CancellationToken ct, IProgress<double> progress)
        {
            await logIn();

            // Check if cancelled in the meantime
            ct.ThrowIfCancellationRequested();

            Stream stream = null;

            try
            {
                stream = await service.GetFileContentsAsync(file, ct, progress);

                if (stream == null)
                {
                    throw new StorageException("Service returned null stream");
                }
            }
            catch (Exception e)
            {
                // Skip OperationCancelExceptions
                if (!(e is OperationCanceledException))
                {
                    string message = string.Format(
                        "Failed downloading file {0}: {1}", file.Name, e.Message);
                    showMessage(message);

                    WaitControl.Finish();
                    NavigationService.GoBack();
                }

                // Throw back the exception so that
                // the call chain won't go on
                throw e;
            }

            // Not cancelled?
            if (ct.IsCancellationRequested)
            {
                stream.Dispose();
                ct.ThrowIfCancellationRequested();
            }

            // Download finished
            downloading = false;

            WaitControl.Finish();

            // Add stream to context
            App.Context.FileStream = stream;

            // Go to AddBookPage
            NavigationService.Navigate(new Uri("/Albite.Reader.App;component/Source/View/Pages/AddBookPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (service == null)
            {
                service = BookStorageServices.GetService(
                    NavigationContext.QueryString["service"]);
                loadingString = "Accessing " + service.Name + "...";
            }

            // Cancel current task (if any)
            cancelCurrentTask();

            // Refresh current folder contents
            loadFolderContents(history.CurrentFolder);

            // currentTask was set by loadFolderContents
            // Update the application bar only after
            // it has finished loading the folder

            currentTask.Task.ContinueWith((continuation) =>
                {
                    // Ready to update application bar
                    initializeApplicationBar();
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private bool appbarInitialized = false;

        private void initializeApplicationBar()
        {
            if (!appbarInitialized)
            {
                // Clear the icon items just in case
                ApplicationBar.Buttons.Clear();

                // Search menu icon
                if (service.IsSearchSupported)
                {
                    ApplicationBarIconButton searchButton = new ApplicationBarIconButton();
                    searchButton.Text = "Search";
                    searchButton.Click += SearchButton_Click;
                    searchButton.IconUri = new Uri("/Resources/Images/feature.search.png", UriKind.Relative);
                    ApplicationBar.Buttons.Add(searchButton);
                }

                // Clear the menu items just in case
                ApplicationBar.MenuItems.Clear();

                // Log out menu button
                if (service.LoginRequired)
                {
                    ApplicationBarMenuItem logoutButton = new ApplicationBarMenuItem("log out");
                    logoutButton.Click += LogoutButton_Click;
                    ApplicationBar.MenuItems.Add(logoutButton);
                }

                if (ApplicationBar.MenuItems.Count > 0 ||  ApplicationBar.Buttons.Count > 0)
                {
                    ApplicationBar.Mode =
                        ApplicationBar.Buttons.Count > 0
                        ? ApplicationBarMode.Default
                        : ApplicationBarMode.Minimized;
                }

                showAppBar();

                appbarInitialized = true;
            }
        }

        private void showAppBar()
        {
            ApplicationBar.IsVisible
                = ApplicationBar.MenuItems.Count > 0
                ||  ApplicationBar.Buttons.Count > 0;
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (service.LoginRequired)
            {
                // cancel any tasks first!
                cancelCurrentTask();

                if (MessageBox.Show(
                    "Do you want to log out?",
                    service.Name,
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    service.LogOut();

                    showMessage("You were logged out.");

                    // Go back to previous page
                    NavigationService.GoBack();
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // Cancel any tasks
            cancelCurrentTask();

            // Hide the app bar (if there)
            ApplicationBar.IsVisible = false;

            // Show the search panel
            SearchPanel.Show();
        }

        private void closeSearch()
        {
            if (SearchPanel.Visibility == Visibility.Visible)
            {
                // Hide the search panel
                SearchPanel.Hide();

                // Show the appbar
                showAppBar();
            }
        }

        private class Progress : IProgress<double>
        {
            BrowsePage page;

            public Progress(BrowsePage page)
            {
                this.page = page;
            }

            public void Report(double value)
            {
                page.WaitControl.Progress = value;
            }
        }

        private void SearchPanel_SearchInitiated(VoiceSearchControl sender, string searchText)
        {
            // Cancel any tasks
            cancelCurrentTask();

            // Close the search
            closeSearch();

            // Get a virtual search folder
            IStorageFolder folder = service.GetSearchFolder(searchText);
            history.GoForward(folder);
        }

        private void SearchPanel_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPanel.SearchInitiated += SearchPanel_SearchInitiated;
        }
    }
}