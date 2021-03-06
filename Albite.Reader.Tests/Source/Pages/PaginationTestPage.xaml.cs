﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Albite.Reader.Engine;
using Albite.Reader.BookLibrary;
using Albite.Reader.Container;
using Albite.Reader.Engine.Layout;
using Albite.Reader.Core.Diagnostics;
using Albite.Reader.BookLibrary.Location;
using System.Windows.Media;
using Albite.Reader.App.View.Pages.BookSettings;

namespace Albite.Reader.Tests.Pages
{
    public partial class PaginationTestPage : PhoneApplicationPage
    {
        public PaginationTestPage()
        {
            InitializeComponent();
        }

        private static readonly string tag = "PaginationTestPage";
        private EnginePresenter presenter;
        private int chapterNumber;
        private int expectedPageCount;

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Log.D(tag, "WebBrowser loaded");
            runTest();
        }

        private void WebBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            presenter.Engine.ReceiveMessage(e.Value);
        }

        private void runTest()
        {
            Library library = new Library(PaginationTest.TestFolder);

            // Should've been added by PaginationTest
            int id = int.Parse(NavigationContext.QueryString["id"]);
            Book book = library.Books[id];

            chapterNumber = int.Parse(NavigationContext.QueryString["chapterNumber"]);
            expectedPageCount = int.Parse(NavigationContext.QueryString["pageCount"]);

            // Get the presenter
            BookPresenter bookPresenter = new BookPresenter(book);

            Log.D(tag, "Creating presenter...");
            presenter = new EnginePresenter(this, bookPresenter);

            Log.D(tag, "Loading book in client...");
            BookLocation location = bookPresenter.Spine[chapterNumber].CreateLocation(
                new DomLocation(new int[] { 0 }, 0, 0));

            presenter.Engine.Navigator.BookLocation = location;
        }

        private int testIterations = 0;

        private void clientLoaded()
        {
            int pageCount = presenter.Engine.Navigator.PageCount;
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

                presenter.ReloadBrowser();
            }
        }

        #region EnginePresenter
        private class EnginePresenter : IEnginePresenter
        {
            private static readonly string tag = "PaginationTestPresenter";

            private readonly PaginationTestPage page;

            public BookPresenter BookPresenter { get; private set; }
            public IEngine Engine { get; private set; }

            public EnginePresenter(PaginationTestPage page, BookPresenter bookPresenter)
            {
                this.page = page;
                Engine = new Albite.Reader.Engine.Engine(this, bookPresenter, DefaultLayoutSettings.LayoutSettings);
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

            public bool ExternalNavigationRequested(Uri uri, string title)
            {
                Log.D(tag, "ExternalNavigationRequested: " + uri + " (" + title + ")");
                return false;
            }

            public bool InternalNavigationApprovalRequested(Uri uri, string title)
            {
                Log.D(tag, "InternalNavigationApprovalRequested: " + uri + " (" + title + ")");
                return false;
            }

            public void NavigationFailed(Uri uri, string title)
            {
                Log.D(tag, "NavigationFailed: " + uri + " (" + title + ")");
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