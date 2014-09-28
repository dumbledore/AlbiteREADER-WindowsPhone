using Albite.Reader.App.View.Controls;
using Albite.Reader.BookLibrary;
using Albite.Reader.BookLibrary.Search;
using Albite.Reader.Core.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using GEArgs = System.Windows.Input.GestureEventArgs;

namespace Albite.Reader.App.View.Pages
{
    public partial class BookSearchPage : PhoneApplicationPage
    {
        private IBookSeeker seeker;
        private CancellableTask currentTask;
        private ObservableCollection<IBookmark> searchResults;

        public BookSearchPage()
        {
            InitializeComponent();

            // Create the collection
            searchResults = new ObservableCollection<IBookmark>();

            // And connect it to the list
            BookmarksList.ItemsSource = searchResults;
        }

        private void cancelCurrentTask()
        {
            if (currentTask != null)
            {
                currentTask.Cancel();
                currentTask = null;
            }

            SystemTray.ProgressIndicator = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the book presenter and create the seeker
            seeker = App.Context.BookPresenter.CreateSeeker();

            // Add event handler for found results
            seeker.ResultsFound += seeker_ResultsFound;

            base.OnNavigatedTo(e);
        }

        void seeker_ResultsFound(object sender, IEnumerable<IBookmark> e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                foreach (IBookmark bookmark in e)
                {
                    searchResults.Add(bookmark);
                }
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Cancel any running tasks
            cancelCurrentTask();

            base.OnNavigatedFrom(e);
        }

        private void BookmarkControl_Tap(object sender, GEArgs e)
        {
            cancelCurrentTask();
        }

        private void SearchControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Add the search handler
            SearchControl.SearchInitiated += SearchControl_SearchInitiated;

            // And focus
            SearchControl.Focus();
        }

        void SearchControl_SearchInitiated(SearchControl sender, string args)
        {
            // Cancel any running tasks
            cancelCurrentTask();

            // Clear currently found results
            searchResults.Clear();

            // Close the keyboard
            BookmarksList.Focus();

            // Hide the results text
            EmptyTextGrid.Visibility = Visibility.Collapsed;

            // Initiate search
            CancellationTokenSource cts = new CancellationTokenSource();
            currentTask = new CancellableTask(search(args, cts.Token), cts);
        }

        private async Task search(string query, CancellationToken ct)
        {
            // Show indication
            ProgressIndicator progress = new ProgressIndicator();
            progress.IsIndeterminate = true;
            progress.IsVisible = true;
            SystemTray.ProgressIndicator = progress;

            await seeker.SearchAsync(query, ct);

            // Indicate search has finished
            SystemTray.ProgressIndicator = null;

            if (searchResults.Count == 0)
            {
                EmptyTextGrid.Visibility = Visibility.Visible;
            }
        }
    }
}