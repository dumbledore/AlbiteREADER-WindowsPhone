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
using Microsoft.Live;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BrowsePage : PhoneApplicationPage
    {
        public BrowsePage()
        {
            InitializeComponent();
        }

        private BrowsingService service = null;
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

            if (e.NavigationMode != NavigationMode.Back)
            {
                // Save the history if not going back
                State[HistoryKey] = history.ToString();
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            if (history.CanGoBack)
            {
                // Go back one step
                history.GoBack();

                // Cancel event, i.e. do not leave page
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        private CancellableTask currentTask;

        private void cancelCurrentTask()
        {
            if (currentTask != null)
            {
                currentTask.Cancel();
                currentTask = null;
            }

            WaitControl.Finish();
        }

        private void FolderControl_Tap(object sender, GEArgs e)
        {
            FolderControl control = (FolderControl)sender;

            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            FolderItem item = control.FolderItem;
            if (item.IsFolder)
            {
                history.GoForward(item);
            }
            else
            {
                MessageBox.Show("Downloading " + item.Name);
            }
        }

        private void loadFolderContents(FolderItem folder)
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

        private async Task loadFolderContentsAsync(FolderItem folder, CancellationToken ct)
        {
            if (service.LoginRequired)
            {
                try
                {
                    await service.LogIn();
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "Failed connecting to " + service.Name,
                        "Error",
                        MessageBoxButton.OK);

                    WaitControl.Finish();
                    NavigationService.GoBack();
                    return;
                }
            }

            // Check if cancelled in the meantime
            ct.ThrowIfCancellationRequested();

            ICollection<FolderItem> items = null;

            try
            {
                items = await service.GetFolderContentsAsync(folder, ct);
            }
            catch (LiveConnectException e)
            {
                MessageBox.Show(
                    "Failed accessing folder: " + e.Message,
                    "Error",
                    MessageBoxButton.OK);

                WaitControl.Finish();
                NavigationService.GoBack();
                return;
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (service == null)
            {
                service = BookBrowsingServices.GetService(
                    NavigationContext.QueryString["service"]);
                loadingString = "Accessing " + service.Name + "...";
            }

            // Cancel current task (if any)
            cancelCurrentTask();

            // Refresh current folder contents
            loadFolderContents(history.CurrentFolder);

            ApplicationBar.IsVisible = service.LoginRequired;
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

                    MessageBox.Show(
                        "You were logged out.",
                        service.Name,
                        MessageBoxButton.OK);

                    // Go back to previous page
                    NavigationService.GoBack();
                }
            }
        }

        private class CancellableTask
        {
            public Task Task { get; private set; }

            private CancellationTokenSource cts;

            public CancellableTask(Task task, CancellationTokenSource cts)
            {
                this.Task = task;
                this.cts = cts;
            }

            public void Cancel()
            {
                if (Task.IsCanceled || Task.IsCompleted)
                {
                    return;
                }

                // Cancel it
                cts.Cancel();

                // Do not wait for it finish!
                // In out case the task is running on the UI thread,
                // but we are waiting for it on the UI thread,
                // which would obviously cause a dead-lock.

                // cts is not needed anymore
                cts = null;
            }
        }
    }
}