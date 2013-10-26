using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Windows;

namespace SvetlinAnkov.Albite.Tests.Pages
{
    public partial class WebBrowserSecurityTestPage : PhoneApplicationPage
    {
        public WebBrowserSecurityTestPage()
        {
            InitializeComponent();
            prepare();
        }

        void prepare()
        {
            // Copy the resources to isolated storage
            copyResToIso("Test/WebBrowser/Security/exploit/exploit.html");
            copyResToIso("Test/WebBrowser/Security/exploit/other.html");
            copyResToIso("Test/WebBrowser/Security/other/other.html");
        }

        void copyResToIso(string filename)
        {
            using (AlbiteIsolatedStorage iso = new AlbiteIsolatedStorage(filename))
            {
                using (AlbiteResourceStorage res = new AlbiteResourceStorage(filename))
                {
                    res.CopyTo(iso);
                }
            }
        }

        private void BadBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            BadBrowser.Source = new Uri("Test/WebBrowser/Security/exploit/exploit.html", UriKind.Relative);
        }

        private static readonly string gbTag = "GoodBrowser";

        private void GoodBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            GoodBrowser.Base = "Test/WebBrowser/Security/exploit/";
            GoodBrowser.Source = new Uri("exploit.html", UriKind.Relative);
        }

        private void GoodBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log.D(gbTag, "Navigated: " + e.Uri.ToString() + ", initiator: " + e.IsNavigationInitiator
                + ", mode: " + e.NavigationMode);
        }

        private void GoodBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            Log.D(gbTag, "Navigating to: " + e.Uri.ToString());
        }

        private void GoodBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Log.E(gbTag, "Navigation failed: " + e.Uri.ToString());
            Log.D(gbTag, "Handling navigation failed");
            e.Handled = true;
        }

        private static readonly string abTag = "AlbiteBrowser";
        private bool cancelNavigation = false;

        private void AlbiteBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            AlbiteBrowser.Base = "Test/WebBrowser/Security/exploit/";
            AlbiteBrowser.Source = new Uri("exploit.html", UriKind.Relative);
        }

        private void AlbiteBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log.D(abTag, "Navigated: " + e.Uri.ToString() + ", initiator: " + e.IsNavigationInitiator
                + ", mode: " + e.NavigationMode);

            // cancel after first navigation
            cancelNavigation = true;
        }

        private void AlbiteBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            Log.D(abTag, "Navigating to: " + e.Uri.ToString());

            if (cancelNavigation)
            {
                e.Cancel = true;
                Log.D(gbTag, "Cancelling navigation");
            }
        }
    }
}