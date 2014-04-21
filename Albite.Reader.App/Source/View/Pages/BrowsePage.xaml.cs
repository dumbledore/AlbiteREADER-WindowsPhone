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

        // TODO handle hibernation:
        // 1. Restore current path
        // 2. Restore browsing service -> Shouldn't it be serializable?

        private BrowsingService service = null;
        private string loadingString = "";

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            cancelCurrentTask();
            base.OnNavigatingFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            if (path.Count > 0)
            {
                // Go up one folder
                goToParentFolder();
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

            IFolderItem item = control.FolderItem;
            if (item.IsFolder)
            {
                goTo(item);
            }
            else
            {
                MessageBox.Show("Downloading " + item.Name);
            }
        }

        private Stack<IFolderItem> path = new Stack<IFolderItem>();

        private void goTo(IFolderItem folder)
        {
            // Update the path
            path.Push(folder);

            // List the folder
            loadFolderContents(folder);
        }

        private void goToParentFolder()
        {
            // First, remove the current folder
            IFolderItem currentFolder = path.Pop();

            // Then get the parent folder
            IFolderItem parent = getCurrentFolder();

            // List the parent folder
            loadFolderContents(parent);
        }

        private void refreshFolderContents()
        {
            loadFolderContents(getCurrentFolder());
        }

        private IFolderItem getCurrentFolder()
        {
            return path.Count > 0 ? path.Peek() : null;
        }

        private void loadFolderContents(IFolderItem folder)
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

        private async Task loadFolderContentsAsync(IFolderItem folder, CancellationToken ct)
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

            ICollection<IFolderItem> items = null;

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

            refreshFolderContents();

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