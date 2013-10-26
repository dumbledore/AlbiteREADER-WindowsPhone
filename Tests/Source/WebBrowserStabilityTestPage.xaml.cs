using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.Core.IO;
using SvetlinAnkov.Albite.Core.Utils;
using System;
using System.Windows;

namespace SvetlinAnkov.Albite.Tests
{
    public partial class WebBrowserStabilityTestPage : PhoneApplicationPage
    {
        private static readonly string baseDir = "Test/WebBrowser/Stability/";

        private static readonly string[] files = new string[] {
            "loop_test.html",
            "allocation_test.html",
            "content_test.html",
        };

        private static readonly string tag = "WebBrowserStability";

        public WebBrowserStabilityTestPage()
        {
            InitializeComponent();
            prepare();
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

        private void prepare()
        {
            foreach (string filename in files)
            {
                copyResToIso(baseDir + filename);
            }
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            Browser.Base = baseDir;
        }

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Log.D(tag, "Navigated: " + e.Uri.ToString() + ", initiator: " + e.IsNavigationInitiator
                + ", mode: " + e.NavigationMode);
        }

        private void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            Log.D(tag, "Navigating to: " + e.Uri.ToString());
        }

        private void Browser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Log.E(tag, "Navigation failed: " + e.Uri.ToString());
            Log.D(tag, "Handling navigation failed");
            e.Handled = true;
        }

        private void Browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Log.D(tag, "ScriptNotify: " + e.Value);
        }

        private void Test1_Click(object sender, RoutedEventArgs e)
        {
            Browser.Source = new Uri("loop_test.html", UriKind.Relative);
        }

        private void Test2_Click(object sender, RoutedEventArgs e)
        {
            Browser.Source = new Uri("allocation_test.html", UriKind.Relative);
        }

        private void Test3_Click(object sender, RoutedEventArgs e)
        {
            Browser.Source = new Uri("content_test.html", UriKind.Relative);
        }
    }
}