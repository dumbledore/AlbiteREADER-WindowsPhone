using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Albite.Reader.App.View.Controls;
using System.Windows.Input;
using Albite.Reader.App.Browse;
using System;
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

        private IBrowsingService service = null;

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

        private async void setCurrentPath(string path)
        {
            try
            {
                await service.LogIn();
            } catch (Exception)
            {
                MessageBox.Show(
                    "Failed connecting to " + service.Name,
                    "Error",
                    MessageBoxButton.OK);

                NavigationService.GoBack();
            }

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

            currentPath = path;
        }

        private void refresh()
        {
            setCurrentPath(currentPath);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (service == null)
            {
                service = BrowsingServices.GetService(
                    NavigationContext.QueryString["service"]);
            }

            refresh();
        }
    }
}