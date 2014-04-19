using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.IO;
using Albite.Reader.App.View.Controls;
using System.Windows.Input;
using Albite.Reader.App.Browse;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BrowsePage : PhoneApplicationPage
    {
        public BrowsePage()
        {
            InitializeComponent();
        }

        //private string currentPath = "/";

        private IBrowsingService browsingService = null;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (browsingService == null)
            {
                browsingService = BrowsingServices.GetService(
                    NavigationContext.QueryString["service"]);
            }

            // TODO handle hibernation:
            // 1. Restore current path
            // 2. Restore browsing service -> Shouldn't it be serializable?

            base.OnNavigatedTo(e);
        }

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
    }
}