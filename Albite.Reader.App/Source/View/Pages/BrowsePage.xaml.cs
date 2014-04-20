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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // TODO cancel async operations
            base.OnNavigatingFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // TODO
            // Three things:
            // 1. Cancel async operations (if any)
            // 2. Go to root folder (if any)
            // 3. Go to previous page

            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            if (path.Count > 0)
            {
                // Go up one folder
                cts = new CancellationTokenSource();
                currentTask = goToParentFolder(cts.Token);
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        private Task currentTask = null;
        private CancellationTokenSource cts = null;

        private void cancelCurrentTask()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }

            if (currentTask != null)
            {
                currentTask.Wait();
                currentTask = null;
            }
        }

        private void FolderControl_Tap(object sender, GEArgs e)
        {
            FolderControl control = (FolderControl)sender;

            // Cancel any previous tasks and wait for them to finish
            cancelCurrentTask();

            IFolderItem item = control.FolderItem;
            if (item.IsFolder)
            {
                cts = new CancellationTokenSource();
                currentTask = goTo(item, cts.Token);
            }
            else
            {
                MessageBox.Show("Downloading " + item.Name);
            }
        }

        private Stack<IFolderItem> path = new Stack<IFolderItem>();

        private async Task goTo(IFolderItem folder, CancellationToken ct)
        {
            await loadFolderContents(folder, ct);
            path.Push(folder);
        }

        private async Task goToParentFolder(CancellationToken ct)
        {
            // First, remove the current folder
            path.Pop();

            // Then update contents
            await refreshFolderContents(ct);
        }

        private async Task refreshFolderContents(CancellationToken ct)
        {
            await loadFolderContents(
                path.Count > 0 ? path.Peek() : null, ct);
        }

        private async Task loadFolderContents(IFolderItem folder, CancellationToken ct)
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

                    NavigationService.GoBack();
                }
            }

            // Check if cancelled in the meantime
            ct.ThrowIfCancellationRequested();

            try
            {
                ICollection<IFolderItem> items = await service.GetFolderContentsAsync(folder, ct);

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
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Failed accessing folder: " + e.Message,
                    "Error",
                    MessageBoxButton.OK);

                NavigationService.GoBack();
            }

            FolderText.Text = folder == null ? service.Name : folder.Name;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (service == null)
            {
                service = BookBrowsingServices.GetService(
                    NavigationContext.QueryString["service"]);
            }

            // Initial state
            FolderText.Text = service.Name;
            FoldersList.ItemsSource = null;

            // Cancel current task (if any)
            cancelCurrentTask();

            cts = new CancellationTokenSource();
            currentTask = refreshFolderContents(cts.Token);

            ApplicationBar.IsVisible = service.LoginRequired;
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (service.LoginRequired)
            {
                if (MessageBox.Show(
                    "Do you want to log out?",
                    service.Name,
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    service.LogOut();

                    // Go back to previous page
                    NavigationService.GoBack();
                }
            }
        }
    }
}