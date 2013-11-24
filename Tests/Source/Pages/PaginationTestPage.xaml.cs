using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SvetlinAnkov.Albite.Engine;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.Container;
using SvetlinAnkov.Albite.Engine.LayoutSettings;
using SvetlinAnkov.Albite.Core.Diagnostics;
using SvetlinAnkov.Albite.BookLibrary.Location;
using System.Windows.Media;

namespace SvetlinAnkov.Albite.Tests.Pages
{
    public partial class PaginationTestPage : PhoneApplicationPage
    {
        public PaginationTestPage()
        {
            InitializeComponent();
        }

        private static readonly string tag = "PaginationTestPage";
        private EngineController controller;
        private int chapterNumber;
        private int expectedPageCount;

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "WebBrowser loaded");
            runTest();
        }

        private void WebBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            controller.Engine.ReceiveMessage(e.Value);
        }

        private void runTest()
        {
            Library library = new Library(PaginationTest.TestFolder);
            // Should've been added by PaginationTest
            Book book = library.Books[1];

            chapterNumber = int.Parse(NavigationContext.QueryString["chapterNumber"]);
            expectedPageCount = int.Parse(NavigationContext.QueryString["pageCount"]);

            // Get the presenter
            BookPresenter bookPresenter = new BookPresenter(book);

            Log.D(tag, "Creating controller...");
            controller = new EngineController(this, bookPresenter);

            Log.D(tag, "Loading book in client...");
            BookLocation location = bookPresenter.Spine[chapterNumber].CreateLocation(
                new DomLocation(new int[] { 0 }, 0));

            controller.Engine.Navigator.BookLocation = location;
        }

        private int testIterations = 0;

        private void clientLoaded()
        {
            int pageCount = controller.Engine.Navigator.PageCount;
            if (pageCount != expectedPageCount)
            {
                string msg = string.Format(
                    "Failed after {0} iterations:\n {1} != {2}",
                    testIterations, pageCount, expectedPageCount);
                Log.D(tag, msg);
                StatusText.Text = msg;
                Color color = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00);
                ContentGrid.Background = new SolidColorBrush(color);
            }
            else
            {
                testIterations++;
                string msg = string.Format(
                    "Run successfully for {0} iterations",
                    testIterations);
                Log.D(tag, msg);
                StatusText.Text = msg;

                controller.ReloadBrowser();
            }
        }

        #region EngineController
        private class EngineController : IEngineController
        {
            private static readonly string tag = "PaginationTestController";

            private readonly PaginationTestPage page;

            public BookPresenter BookPresenter { get; private set; }
            public AlbiteEngine Engine { get; private set; }

            public EngineController(PaginationTestPage page, BookPresenter bookPresenter)
            {
                this.page = page;
                Engine = new AlbiteEngine(this, bookPresenter, new Settings());
            }

            public int Width
            {
                get { return (int)page.WebBrowser.ActualWidth; }
            }

            public int Height
            {
                get { return (int)page.WebBrowser.ActualHeight; }
            }

            public string BasePath
            {
                get { return page.WebBrowser.Base; }
                set { page.WebBrowser.Base = value; }
            }

            public void ReloadBrowser()
            {
                LoadingStarted();
                page.WebBrowser.Navigate(Engine.Uri);
            }

            public bool IsLoading { get; private set; }

            public string SendMessage(string message)
            {
                Log.D(tag, "SendMessage: " + message);

                if (IsLoading)
                {
                    Log.D(tag, "Can't send command. Still loading.");
                    return null;
                }

                return (string)page.WebBrowser.InvokeScript(
                    "albite_notify", new string[] { message });
            }

            public void LoadingStarted()
            {
                IsLoading = true;
            }

            public void LoadingCompleted()
            {
                IsLoading = false;
                page.clientLoaded();
            }

            public void NavigationRequested(string url)
            {
                Log.D(tag, "Navigation: " + url);
            }

            public void OnError(string message)
            {
                Log.E(tag, "ReaderError: " + message);
            }

            public int ApplicationBarHeight
            {
                get { return 0; }
            }
        }
        #endregion
    }
}