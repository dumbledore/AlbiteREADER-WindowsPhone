using Microsoft.Phone.Controls;
using SvetlinAnkov.Albite.BookLibrary;
using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.READER.View.Controls;
using System;
using System.Windows.Navigation;

namespace SvetlinAnkov.Albite.READER.View.Pages
{
    public partial class BookmarksPage : PhoneApplicationPage
    {
        public BookmarksPage()
        {
            InitializeComponent();
        }

        private Bookmark[] bookmarks;

        private void setCurrentState()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Set the title
            string titleUppercase = bookPresenter.Book.Title.ToUpper();
            PageTitle.Text = titleUppercase;

            // Get all current bookmakrs
            bookmarks = bookPresenter.BookmarkManager.GetAll();

            // Sort them so that they are listed correctly
            Array.Sort(bookmarks);

            // Fill the bookmarks
            foreach (Bookmark bookmark in bookmarks)
            {
                HeaderedContentControl control = new HeaderedContentControl();
                control.HeaderText = "14%";
                control.ContentText = bookmark.Text;
                BookmarksList.Children.Add(control);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the current state
            setCurrentState();

            // Go on as usual
            base.OnNavigatedTo(e);
        }

#if false
        private void applyLocation()
        {
            // Get the context
            AlbiteContext context = ((IAlbiteApplication)App.Current).CurrentContext;

            // Get the book presenter
            BookPresenter bookPresenter = context.BookPresenter;

            // Get new location
            int currentLocation = (int)LocationSlider.Value;

            if (initialLocation != currentLocation)
            {
                // Get the chapter number
                int chapterNumber = currentLocation / stepsPerChapter;

                // Get the offset
                double chapterOffset = (currentLocation % stepsPerChapter) / ((double)(stepsPerChapter - 1));

                // Get the chapter
                Chapter chapter = bookPresenter.Spine[chapterNumber];

                // Create the RelativeLocation
                RelativeChapterLocation relativeLocation = new RelativeChapterLocation(chapterOffset);

                // Create the BookLocation
                BookLocation location = chapter.CreateLocation(relativeLocation);

                // Set the new location
                bookPresenter.BookLocation = location;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // First apply the new settings
            applyLocation();

            // Go on as usual
            base.OnNavigatingFrom(e);
        }

        private void GoToButton_Click(object sender, EventArgs e)
        {
            // Simply go back to the ReaderPage
            NavigationService.GoBack();
        }
#endif
    }
}