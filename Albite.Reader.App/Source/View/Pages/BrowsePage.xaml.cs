using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Albite.Reader.App.View.Controls;
using System.Windows.Input;
using Albite.Reader.App.Browse;
using System;
using System.Threading.Tasks;
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
            base.OnBackKeyPress(e);
        }

        private void FolderControl_Tap(object sender, GEArgs e)
        {
            FolderControl control = (FolderControl)sender;

            // TODO
        }

        private string currentPath = "/";

        private async Task setCurrentPath(string path)
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

            try
            {
                ICollection<IFolderItem> items = await service.GetFolderContentsAsync(path);

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
            catch (Exception)
            {
                MessageBox.Show(
                    "Failed accessing folder",
                    "Error",
                    MessageBoxButton.OK);

                NavigationService.GoBack();
            }

            currentPath = path;
        }

        private async Task refresh()
        {
            await setCurrentPath(currentPath);
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (service == null)
            {
                service = BookBrowsingServices.GetService(
                    NavigationContext.QueryString["service"]);
            }

            await refresh();

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